using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForeCast;
using WeatherForeCast.Controllers;
using WeatherForeCast.Services;

[TestFixture]
public class WeatherForecastControllerTests
{
    private Mock<ILogger<WeatherForecastController>> _loggerMock;
    private Mock<IWeatherDataProvider> _weatherDataProviderMock;
    private Mock<IJsonProcessor> _jsonProcessorMock;
    private WeatherForecastController _controller;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<WeatherForecastController>>();
        _weatherDataProviderMock = new Mock<IWeatherDataProvider>();
        _jsonProcessorMock = new Mock<IJsonProcessor>();
        _controller = new WeatherForecastController(_loggerMock.Object, _weatherDataProviderMock.Object, _jsonProcessorMock.Object);
    }

    [Test]
    public void GetCurrentReturnsNotFoundResultIfWeatherDataProviderFails()
    {
        // Arrange
        var weatherData = "{}";
        _weatherDataProviderMock.Setup(x => x.GetCurrentWeather(It.IsAny<double>(), It.IsAny<double>())).Throws(new Exception());

        // Act
        var result = _controller.GetCurrent();

        // Assert
        Assert.IsInstanceOf(typeof(NotFoundObjectResult), result.Result);
    }

    [Test]
    public void GetCurrentReturnsNotFoundResultIfWeatherDataIsInvalid()
    {
        // Arrange
        var weatherData = "{}";
        _weatherDataProviderMock.Setup(x => x.GetCurrentWeather(It.IsAny<double>(), It.IsAny<double>())).Returns(weatherData);
        _jsonProcessorMock.Setup(x => x.Process(weatherData)).Throws<Exception>();

        // Act
        var result = _controller.GetCurrent();

        // Assert
        Assert.IsInstanceOf(typeof(NotFoundObjectResult), result.Result);
    }

    [Test]
    public void GetCurrentReturnsOkResultIfWeatherDataIsValid()
    {
        // Arrange
        var expectedForecast = new WeatherForecast();
        var weatherData = "{}";
        _weatherDataProviderMock.Setup(x => x.GetCurrentWeather(It.IsAny<double>(), It.IsAny<double>())).Returns(weatherData);
        _jsonProcessorMock.Setup(x => x.Process(weatherData)).Returns(expectedForecast);

        // Act
        var result = _controller.GetCurrent();

        // Assert
        Assert.IsInstanceOf(typeof(OkObjectResult), result.Result);
        Assert.That(((OkObjectResult)result.Result).Value, Is.EqualTo(expectedForecast));
    }
}