using System.Net;

namespace WeatherForeCast.Services
{
    public class OpenWeatherMapApi : IWeatherDataProvider
    {
        private readonly ILogger<OpenWeatherMapApi> _logger;

        public OpenWeatherMapApi(ILogger<OpenWeatherMapApi> logger)
        {
            _logger = logger;
        }
        public string GetCurrentWeather(double lat, double lon)
        {
            var apiKey = "410b77bb7ea5e4483af51a593d71c09d";
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

            var client = new WebClient();

            _logger.LogInformation("Calling OpenWeather API with url: {url}", url);
            return client.DownloadString(url);
        }
    }
}

