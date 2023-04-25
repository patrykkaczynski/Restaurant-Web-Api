using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantDto> GetByIdAsync(int id);
        Task<IEnumerable<RestaurantDto>> GetAllAsync();
        Task<int> CreateAsync(CreateRestaurantDto dto);
        Task UpdateAsync(int id, UpdateRestaurantDto dto);
        Task DeleteAsync(int id);
       
    }
}