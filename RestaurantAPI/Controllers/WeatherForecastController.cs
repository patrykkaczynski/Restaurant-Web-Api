using Microsoft.AspNetCore.Mvc;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _service;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("generate")]
        public ActionResult<IEnumerable<WeatherForecast>> Generate([FromQuery] int count, [FromBody] TemperatureRequest request)
        {
            if (count < 0 || request.Max < request.Min)
            {
                return BadRequest();
            }
            var result = _service.Get(count, request.Min, request.Max);
            return Ok(result);
        }



        //mo¿liwe jest okreœlenie adresu do danej akcji poprzez atrybut Route lub poprzez konstruktor atrybutu z czasownikiem HTTP
        //[HttpGet("currentDay/{max}")]
        ////[Route("currentDay")]
        //public IEnumerable<WeatherForecast> Get2([FromQuery]int take, [FromRoute]int max)
        //{
        //    var result = _service.Get();
        //    return result;
        //}

        // Akcja to publiczna metoda w kontrolerze, która bêdzie odpowiadaæ na zapytania wysy³ane przez klientów HTTP
        [HttpPost]
        public ActionResult<string> Hello([FromBody]string name)
        {
            //HttpContext to w³aœnoœæ ka¿dego kontrolera przez który jest dostêp zarówno do zapytania jak i odpowiedzi HTTP 
            //HttpContext.Response.StatusCode = 401; - 1 sposób ustawienia kodu statusu dzia³a do wersji .NET5

            //return StatusCode(401, $"Hello {name}"); - 2 sposób ustawienia kodu statusu

            return NotFound($"Hello {name}"); //3 sposób ustawienia kodu statusu
        }
    }
}