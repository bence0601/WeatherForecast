using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
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
        public WeatherForecast Getcurrent()
        {
            var apiKey = "410b77bb7ea5e4483af51a593d71c09d";

            var lat = 47.497913;
            var lon = 19.040236;
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

            var client = new WebClient();

            _logger.LogInformation("Calling OpenWeather API with url : {url}", url);
            var weatherData = client.DownloadString(url);

            return ProcessJsonResponse(weatherData);
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