namespace WeatherForeCast.Services
{
    public interface IJsonProcessor
    {
        WeatherForecast Process(string data);
    }
}
