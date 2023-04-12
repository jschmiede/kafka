using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

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

            try {
                var result = await client.GetAsync("api/faulty");
                result.EnsureSuccessStatusCode();
                if (result.StatusCode == System.Net.HttpStatusCode.NoContent) {
                    return NoContent();
                }
                var body = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(body);
                return Ok(response);


            } catch (HttpRequestException ex) {
                return BadRequest(ex);
            }


        }

        [HttpPost]
        public async Task Post([FromBody] Weather weather) {

            var response = await WeaderDataPublisher.PublishAsync(weather);
            Logger.LogInformation($"Value: {response.Value}, TopicPartitionOffset: {response.TopicPartitionOffset}, Timestamp: {response.Timestamp.UtcDateTime:HH:mm:ss.fff}, Status: {response.Status}");

        }

    }
}