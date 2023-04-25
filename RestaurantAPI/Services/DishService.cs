using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> CreateAsync(int restaurantId, CreateDishDto dto)
        {
            var restaurant = await GetRestaurantByIdAsync(restaurantId);

            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId = restaurantId;

            await _context.Dishes.AddAsync(dishEntity);
            await _context.SaveChangesAsync();

            return dishEntity.Id;
        }

        public async Task<DishDto> GetByIdAsync(int restaurantId, int dishId)
        {
            var restaurant = await GetRestaurantByIdAsync(restaurantId);

            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);

            if (dish is null || dish.RestaurantId != restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }

            var dishDto = _mapper.Map<DishDto>(dish);

            return dishDto;

        }

        public async Task<List<DishDto>> GetAllAsync(int restaurantId)
        {
            var restaurant = await GetRestaurantByIdAsync(restaurantId);


            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishDtos;
        }
        public async Task RemoveAllAsync(int restaurantId)
        {
            var restaurant = await GetRestaurantByIdAsync(restaurantId);

            _context.RemoveRange(restaurant.Dishes);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveByIdAsync(int restaurantId, int dishId)
        {
            var restaurant = await GetRestaurantByIdAsync(restaurantId);

            var dish = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);

            if (dish is null || dish.RestaurantId != restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
        }

        private async Task<Restaurant> GetRestaurantByIdAsync(int restaurantId)
        {
            var restaurant = await _context
               .Restaurants
               .Include(r => r.Dishes)
               .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            return restaurant;
        }
    }
}
