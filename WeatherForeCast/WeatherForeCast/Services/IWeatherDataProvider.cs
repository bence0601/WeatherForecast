namespace WeatherForeCast.Services
{
    public interface IWeatherDataProvider
    {
        string GetCurrentWeather(double lat, double lon);
    }
}
