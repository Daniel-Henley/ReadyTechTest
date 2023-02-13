using ReadyTechTest.API.Controllers;
using ReadyTechTest.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ReadyTechTest.API.Services;

namespace ReadyTechTest.Controllers.Tests;

public class BrewCoffeeTests
{
    readonly static DateTime aprFirst = new DateTime(2023, 4, 1);
    readonly static DateTime janFirst = new DateTime(2023, 1, 1);

    [InlineData("2023/1/1")]
    [InlineData("2023/4/2")]
    [Theory]
    public async void OnNonAprilFirstBrewCoffeeReturnsExpectedAsync(string date)
    {
        var mockLogger = new Mock<ILogger<CoffeeController>>();
        var mockWeatherService = new Mock<IWeatherService>();
        mockWeatherService.Setup(ws => ws.GetWeatherByLocationAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync(20);

        var dateTime = DateTime.Parse(date);
        SystemTime.SetDateTime(dateTime);

        var expectedResponse = new CoffeeResponse(Constants.CoffeeResponseText, dateTime);

        var coffeeController = new CoffeeController(mockLogger.Object, mockWeatherService.Object);
        var response = await coffeeController.BrewCoffeeAsync();

        response.Value.ShouldBe(expectedResponse);
        mockWeatherService.VerifyAll();
    }

    [InlineData("2023/1/1")]
    [InlineData("2023/4/2")]
    [Theory]
    public async void OnNonAprilFirstAbove30cBrewCoffeeReturnsExpectedAsync(string date)
    {
        var mockLogger = new Mock<ILogger<CoffeeController>>();
        var mockWeatherService = new Mock<IWeatherService>();
        mockWeatherService.Setup(ws => ws.GetWeatherByLocationAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync(31);

        var dateTime = DateTime.Parse(date);
        SystemTime.SetDateTime(dateTime);

        var expectedResponse = new CoffeeResponse(Constants.IcedCoffeeResponseText, dateTime);

        var coffeeController = new CoffeeController(mockLogger.Object, mockWeatherService.Object);
        var response = await coffeeController.BrewCoffeeAsync();

        response.Value.ShouldBe(expectedResponse);
        mockWeatherService.VerifyAll();
    }

    [Fact]
    public async void OnAprilFirstBrewCoffeeReturnsImATeapotAsync()
    {
        var mockLogger = new Mock<ILogger<CoffeeController>>();
        var mockWeatherService = new Mock<IWeatherService>();
        SystemTime.SetDateTime(aprFirst);

        var expectedResponse = new StatusCodeResult(StatusCodes.Status418ImATeapot);

        var coffeeController = new CoffeeController(mockLogger.Object, mockWeatherService.Object);
        var response = await coffeeController.BrewCoffeeAsync();

        response.Result.ShouldBeEquivalentTo(expectedResponse);
        mockWeatherService.VerifyAll();
    }

    [Fact]
    public async void OnFifthCallBrewCoffeeReturnsServiceUnavailableAsync()
    {
        var mockLogger = new Mock<ILogger<CoffeeController>>();
        var mockWeatherService = new Mock<IWeatherService>();
        mockWeatherService.Setup(ws => ws.GetWeatherByLocationAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync(20);

        SystemTime.SetDateTime(janFirst);

        var expectedResponse = new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        var coffeeController = new CoffeeController(mockLogger.Object, mockWeatherService.Object);
        Counter.ResetCount();
        for (int i = 0; i < 4; i++)
        {
            await coffeeController.BrewCoffeeAsync();
        }

        var response = await coffeeController.BrewCoffeeAsync();

        response.Result.ShouldBeEquivalentTo(expectedResponse);
        mockWeatherService.VerifyAll();
    }
}