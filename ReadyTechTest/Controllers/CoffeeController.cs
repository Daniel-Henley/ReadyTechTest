using Microsoft.AspNetCore.Mvc;
using ReadyTechTest.API.Shared;
using ReadyTechTest.API.Models;
using Swashbuckle.AspNetCore.Annotations;
using Newtonsoft.Json;
using Swashbuckle.Swagger;
using ReadyTechTest.API.Services;

namespace ReadyTechTest.API.Controllers;

[ApiController]
public class CoffeeController : ControllerBase
{
    private readonly ILogger<CoffeeController> _logger;
    private readonly IWeatherService _weatherService;    

    //would configure in appsettings if likely to change
    private const int maxCount = 5;

    public CoffeeController(ILogger<CoffeeController> logger, IWeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
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
        //would set these up as a appsetting
        var lat = "-37.81";
        var lon = "144.96";
        var currentTemp = await _weatherService.GetWeatherByLocationAsync(lat, lon);

        var response = currentTemp > 30f ? new CoffeeResponse(Constants.IcedCoffeeResponseText, date)
                                         : new CoffeeResponse(Constants.CoffeeResponseText, date);

        return response;
    }
}
