using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
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

            await _restaurantService.Update(id, dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _restaurantService.Delete(id);

            return NotFound();
        }


        [HttpPost]
        public async Task<ActionResult> CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {

            var id = await _restaurantService.Create(dto);

            return Created($"/api/restaurant/{id}", null);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetAll()
        {
            var restaurantDtos = await _restaurantService.GetAll();

            return Ok(restaurantDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> Get([FromRoute] int id)
        {
            var restaurant = await _restaurantService.GetById(id);

            return Ok(restaurant);
        }
    }
}
