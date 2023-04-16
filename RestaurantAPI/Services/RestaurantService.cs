using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> Update(int id, UpdateRestaurantDto dto)
        {
            var restaurant = await _dbContext
              .Restaurants
              .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant is null)
            {
                return false;
            }

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            await _dbContext.SaveChangesAsync();

            return true;

        }

        public async Task<bool> Delete(int id)
        {
            var restaurant = await _dbContext
              .Restaurants
              .FirstOrDefaultAsync(r => r.Id == id);

            if(restaurant is null)
            {
                return false;
            }

            _dbContext.Restaurants.Remove(restaurant);
            await _dbContext.SaveChangesAsync();

            return true;

        }

        public async Task<RestaurantDto> GetById(int id)
        {
            var restaurant = await _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .FirstOrDefaultAsync(r => r.Id == id);

            if (restaurant is null)
            {
                return null;
            }

            var result = _mapper.Map<RestaurantDto>(restaurant);

            return result;
        }

        public async Task<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurants = await _dbContext
               .Restaurants
               .Include(r => r.Address)
               .Include(r => r.Dishes)
               .ToListAsync();

            #region Mapping By Select
            //var restuarantDtos = restaurants.Select(r => new RestaurantDto()
            //{
            //    Name = r.Name,
            //    Category = r.Category,
            //    City = r.Address.City
            //});
            #endregion

            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            return restaurantDtos;
        }

        public async Task<int> Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            await _dbContext.Restaurants.AddAsync(restaurant);
            await _dbContext.SaveChangesAsync();

            return restaurant.Id;
        }
    }

}

