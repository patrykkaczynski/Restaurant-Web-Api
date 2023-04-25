using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        Task<int> CreateAsync(int restaurantId, CreateDishDto dto);
        Task<DishDto> GetByIdAsync(int restaurantId, int dishId);
        Task<List<DishDto>> GetAllAsync(int restaurantId);
        Task RemoveAllAsync(int restaurantId);
        Task RemoveByIdAsync(int restaurantId, int dishId);
    }
}