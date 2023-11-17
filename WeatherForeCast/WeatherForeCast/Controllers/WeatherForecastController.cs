using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherDataProvider weatherDataProvider)
        {
            _logger = logger;
            _weatherDataProvider = weatherDataProvider;
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
        public ActionResult<WeatherForecast> Getcurrent()
        {
            var apiKey = "410b77bb7ea5e4483af51a593d71c09d";

            var lat = 47.497913;
            var lon = 19.040236;
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";
            try
            {
                var weatherData = _weatherDataProvider.GetCurrentWeather(lat, lon);
                return ProcessJsonResponse(weatherData);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting weather data");
                return NotFound("Error getting weather data");
            }
        }
        private static WeatherForecast ProcessJsonResponse(string weatherData)
        {
            JsonDocument json = JsonDocument.Parse(weatherData);
            JsonElement main = json.RootElement.GetProperty("main");
            JsonElement weather = json.RootElement.GetProperty("weather")[0];

            WeatherForecast forecast = new WeatherForecast
            {
                Date = GetDateTimeFromUnixTimeStamp(json.RootElement.GetProperty("dt").GetInt64()),
                TemperatureC = (int)main.GetProperty("temp").GetDouble(),
                Summary = weather.GetProperty("description").GetString()
            };

            return forecast;
        }
        private static DateTime GetDateTimeFromUnixTimeStamp(long timeStamp)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
            DateTime dateTime = dateTimeOffset.UtcDateTime;

            return dateTime;
        }
    }
}