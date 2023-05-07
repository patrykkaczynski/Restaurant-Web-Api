using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
        {
            var newDishId = await _dishService.CreateAsync(restaurantId, dto);

            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);
        }


        [HttpGet("{dishId}")]
        public async Task<ActionResult<DishDto>> Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = await _dishService.GetByIdAsync(restaurantId, dishId);

            return Ok(dish);
        }

        [HttpGet]
        public async Task<ActionResult<DishDto>> GetAll([FromRoute] int restaurantId)
        {
            var result = await _dishService.GetAllAsync(restaurantId);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromRoute] int restaurantId)
        {
            await _dishService.RemoveAllAsync(restaurantId);

            return NoContent();
        }


        [HttpDelete("{dishId}")]
        public async Task<ActionResult> DeleteById([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            await _dishService.RemoveByIdAsync(restaurantId, dishId);

            return NoContent();
        }

    }
}
