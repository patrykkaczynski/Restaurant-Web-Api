using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantDto> GetById(int id);
        Task<IEnumerable<RestaurantDto>> GetAll();
        Task<int> Create(CreateRestaurantDto dto);
        Task Update(int id, UpdateRestaurantDto dto);
        Task Delete(int id);
       
    }
}