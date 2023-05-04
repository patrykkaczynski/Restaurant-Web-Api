using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantDto> GetByIdAsync(int id);
        Task<PagedResult<RestaurantDto>> GetAllAsync(RestaurantQuery query);
        Task<int> CreateAsync(CreateRestaurantDto dto);
        Task UpdateAsync(int id, UpdateRestaurantDto dto);
        Task DeleteAsync(int id);
       
    }
}