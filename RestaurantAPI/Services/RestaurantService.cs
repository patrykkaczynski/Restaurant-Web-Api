using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _auhtorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger
            , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _auhtorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public async Task UpdateAsync(int id, UpdateRestaurantDto dto)
        {

            var restaurant = await _dbContext
              .Restaurants
              .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = await _auhtorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update));

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Update of this resource is forbidden");
            }

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            await _dbContext.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogError($"Restaurant with: {id} DELETE action invoked");

            var restaurant = await _dbContext
              .Restaurants
              .FirstOrDefaultAsync(r => r.Id == id);

            if(restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = await _auhtorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete));

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Delete of this resource is forbidden");
            }

            _dbContext.Restaurants.Remove(restaurant);
            await _dbContext.SaveChangesAsync();


        }

        public async Task<RestaurantDto> GetByIdAsync(int id)
        {
            var restaurant = await _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var result = _mapper.Map<RestaurantDto>(restaurant);

            return result;
        }

        public async Task<PagedResult<RestaurantDto>> GetAllAsync(RestaurantQuery query)
        {
            var baseQuery = _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                                                    || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));

            if(!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    { nameof(Restaurant.Name), r => r.Name },
                    { nameof(Restaurant.Description), r => r.Description },
                    { nameof(Restaurant.Category), r => r.Category}
                };

                var selectedColumn = columnsSelectors[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = await baseQuery
               .Skip(query.PageSize * (query.PageNumber - 1))
               .Take(query.PageSize)
               .ToListAsync();

            var totalItemsCount = baseQuery.Count();
            #region Mapping By Select
            //var restuarantDtos = restaurants.Select(r => new RestaurantDto()
            //{
            //    Name = r.Name,
            //    Category = r.Category,
            //    City = r.Address.City
            //});
            #endregion

            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public async Task<int> CreateAsync(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId;
            await _dbContext.Restaurants.AddAsync(restaurant);
            await _dbContext.SaveChangesAsync();

            return restaurant.Id;
        }
    }

}

