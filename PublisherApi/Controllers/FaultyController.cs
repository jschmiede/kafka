using Microsoft.AspNetCore.Mvc;

using ShartedLib;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PublisherApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class FaultyController : ControllerBase {
        private readonly IWeatherForecastService WeatherForecastService;

        public FaultyController(IWeatherForecastService weatherForecastService) {
            WeatherForecastService = weatherForecastService;
        }


        // GET: api/<FaultyController>
        [HttpGet]
        public ActionResult<IEnumerable<WeatherForecast>> Get() {
            var now = DateTime.Now;
            switch (now.Millisecond % 3) {
                case 0:
                    return Ok(WeatherForecastService.GetForcasts());
                case 1:
                    return base.NoContent();
                case 2:
                    return StatusCode(StatusCodes.Status500InternalServerError);
                default:
                    return NotFound();
            }

        }

        //// GET api/<FaultyController>/5
        //[HttpGet("{id}")]
        //public string Get(int id) {
        //    return "value";
        //    }

        //// POST api/<FaultyController>
        //[HttpPost]
        //public void Post([FromBody] string value) {
        //    }

        //// PUT api/<FaultyController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value) {
        //    }

        //// DELETE api/<FaultyController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id) {
        //    }
    }
}
