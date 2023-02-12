using ReadyTechTest.API.Controllers;
using ReadyTechTest.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ReadyTechTest.Controllers.Tests;

public class BrewCoffeeTests
{
    readonly static DateTime aprFirst = new DateTime(2023, 4, 1);
    readonly static DateTime janFirst = new DateTime(2023, 1, 1);

    [InlineData("2023/1/1")]
    [InlineData("2023/4/2")]
    [Theory]
    public void OnNonAprilFirstBrewCoffeeReturnsExpected(string date)
    {
        var mockLogger = new Mock<ILogger<CoffeeController>>();            
        var dateTime = DateTime.Parse(date);
        SystemTime.SetDateTime(dateTime);

        var expectedResponse = new CoffeeResponse(Constants.CoffeeResponseText, dateTime);

        var coffeeController = new CoffeeController(mockLogger.Object);
        var response = coffeeController.BrewCoffee();

        response.Value.ShouldBe(expectedResponse);
    }
    [Fact]
    public void OnAprilFirstBrewCoffeeReturnsImATeapot()
    {
        var mockLogger = new Mock<ILogger<CoffeeController>>();
        SystemTime.SetDateTime(aprFirst);

        var expectedResponse = new StatusCodeResult(StatusCodes.Status418ImATeapot);

        var coffeeController = new CoffeeController(mockLogger.Object);
        var response = coffeeController.BrewCoffee();

        response.Result.ShouldBeEquivalentTo(expectedResponse);
    }

    [Fact]
    public void OnFifthCallBrewCoffeeReturnsServiceUnavailable()
    {
        var mockLogger = new Mock<ILogger<CoffeeController>>();
        SystemTime.SetDateTime(janFirst);

        var expectedResponse = new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);

        var coffeeController = new CoffeeController(mockLogger.Object);
        for(int i = 0; i < 4; i++) 
        {
            coffeeController.BrewCoffee();
        }
        var response = coffeeController.BrewCoffee();

        response.Result.ShouldBeEquivalentTo(expectedResponse);
    }
}