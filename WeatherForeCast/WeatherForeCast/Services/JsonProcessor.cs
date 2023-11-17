using System.Text.Json;

namespace WeatherForeCast.Services
{
    public class JsonProcessor : IJsonProcessor
    {
        public WeatherForecast Process(string data)
        {
            JsonDocument json = JsonDocument.Parse(data);
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
