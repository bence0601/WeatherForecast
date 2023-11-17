using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WeatherForeCast.Services;

namespace WeatherForeCast.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherDataProvider _weatherDataProvider;
        private readonly IJsonProcessor _jsonProcessor;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherDataProvider weatherDataProvider, IJsonProcessor jsonProcessor)
        {
            _logger = logger;
            _weatherDataProvider = weatherDataProvider;
            _jsonProcessor = jsonProcessor;
        }
        [HttpGet]
        [Route("GetWeatherForecast")]
        public ActionResult<IEnumerable<WeatherForecast>> Get([Required] DateTime date, [Required] string city)
        {
            if (date.Year > 2023)
            {
                return NotFound("Invalid date. Please provide a date before 2023.");
            }
            _logger.LogInformation("Received a request for weather forecast");

            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = date.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                City = city
            })
                .ToArray();

            return Ok();
        }

        [HttpGet("GetCurrent")]
        public ActionResult<WeatherForecast> GetCurrent()
        {
            var lat = 47.497913;
            var lon = 19.040236;

            try
            {
                var weatherData = _weatherDataProvider.GetCurrentWeather(lat, lon);
                return Ok(_jsonProcessor.Process(weatherData));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting weather data");
                return NotFound("Error getting weather data");
            }
        }

    }
}