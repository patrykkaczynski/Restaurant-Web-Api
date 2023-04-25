using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        Task<int> Create(int restaurantId, CreateDishDto dto);
        Task<DishDto> GetById(int restaurantId, int dishId);
        Task<List<DishDto>> GetAll(int restaurantId);
        Task RemoveAll(int restaurantId);
        Task RemoveById(int restaurantId, int dishId);
    }
}