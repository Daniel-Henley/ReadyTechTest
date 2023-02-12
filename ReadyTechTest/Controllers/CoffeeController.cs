using Microsoft.AspNetCore.Mvc;
using ReadyTechTest.API.Shared;
using ReadyTechTest.API.Models;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;
using Swashbuckle.Swagger;

namespace ReadyTechTest.API.Controllers;

[ApiController]
public class CoffeeController : ControllerBase
{
    private readonly ILogger<CoffeeController> _logger;
    //would configure in appsettings if likely to change
    private const int maxCount = 5;

    public CoffeeController(ILogger<CoffeeController> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    [SwaggerResponse(StatusCodes.Status200OK,
                     "Coffee was brewed!",
                     typeof(CoffeeResponse),
                     Constants.ContentTypeApplicationJson)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable,
                     "Coffee machine is out of coffee")]
    [SwaggerResponse(StatusCodes.Status418ImATeapot,
                     "April fools!")]
    [HttpGet("brew-coffee")]
    public async Task<ActionResult<CoffeeResponse>> BrewCoffeeAsync()
    {
        //I'd generally use something like Mediatr to handle requests, but doesn't seem worthwhile for one endpoint
        var date = SystemTime.Now();
        if (date.Month == Constants.April && date.Day == Constants.First)
        {
            return new StatusCodeResult(StatusCodes.Status418ImATeapot);
        }

        var count = Counter.IncrementCount();
        if (count == maxCount)
        {
            Counter.ResetCount();
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }

        //generally put this in as a user secret
        var apiKey = "a0de4d9ee194eb716c8e52c5046952af";
        //would set these up as a appsetting
        var lat = -37.81;
        var lon = 144.96;

        var queryString = $"weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

        _httpClient.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
        var weatherResponse = await _httpClient.GetStringAsync(queryString);

        //for most cases i'd use non anonymous objects but we only need 1 field from the response
        dynamic weather = JsonConvert.DeserializeObject(weatherResponse);
        float currentTemp = weather.main.temp;
        var response = currentTemp > 30f ? new CoffeeResponse(Constants.IcedCoffeeResponseText, date)
                                        : new CoffeeResponse(Constants.CoffeeResponseText, date);

        return response;
    }
}
