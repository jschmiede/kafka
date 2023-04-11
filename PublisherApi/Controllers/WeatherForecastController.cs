using Microsoft.AspNetCore.Mvc;

using ShartedLib;

namespace PublisherApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase {


        private readonly ILogger<WeatherForecastController> Logger;
        private readonly IWeaderDataPublisher WeaderDataPublisher;
        private readonly IHttpClientFactory HttpClientFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeaderDataPublisher weaderDataPublisher, IHttpClientFactory httpClientFactory) {
            Logger = logger;
            WeaderDataPublisher = weaderDataPublisher;
            HttpClientFactory = httpClientFactory;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> GetAsync() {
            var client = HttpClientFactory.CreateClient("ApiClient");
            var result = await client.GetFromJsonAsync<IEnumerable<WeatherForecast>>("api/faulty");
            if (result != null) {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task Post([FromBody] Weather weather) {

            var response = await WeaderDataPublisher.PublishAsync(weather);
            Logger.LogInformation($"Value: {response.Value}, TopicPartitionOffset: {response.TopicPartitionOffset}, Timestamp: {response.Timestamp.UtcDateTime:HH:mm:ss.fff}, Status: {response.Status}");

        }

    }
}