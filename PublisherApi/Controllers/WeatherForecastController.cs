using Microsoft.AspNetCore.Mvc;

using ShartedLib;

namespace PublisherApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeaderDataPublisher WeaderDataPublisher;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeaderDataPublisher weaderDataPublisher) {
            _logger = logger;
            WeaderDataPublisher = weaderDataPublisher;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get() {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public async Task Post([FromBody] Weather weather) {

            var response = await WeaderDataPublisher.PublishAsync(weather);
            _logger.LogInformation($"Value: {response.Value}, TopicPartitionOffset: {response.TopicPartitionOffset}, Timestamp: {response.Timestamp.UtcDateTime:HH:mm:ss.fff}, Status: {response.Status}");

        }

    }
}