using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] UpdateRestaurantDto dto)
        {
            #region ModelState
            //jesli do akcji przyjdzie jakiekolwiek zapytanie dla ktorego istnieje walidacja modelu za pomocą atrybutu ApiController,
            //to kod poniżej zostanie wywołany automatycznie, przez co można pozbyć się z akcji
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            #endregion

            await _restaurantService.UpdateAsync(id, dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _restaurantService.DeleteAsync(id);

            return NoContent();
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var id = await _restaurantService.CreateAsync(dto);

            return Created($"/api/restaurant/{id}", null);
        }

        [HttpGet]
        [Authorize(Policy = "CreatedAtleast2Restaurants")]
        //[Authorize(Policy = "Atleast20")]
        //[Authorize(Policy = "HasNationality")]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll()
        {
            var restaurantDtos = await _restaurantService.GetAllAsync();

            return Ok(restaurantDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RestaurantDto>> Get([FromRoute] int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);

            return Ok(restaurant);
        }
    }
}
