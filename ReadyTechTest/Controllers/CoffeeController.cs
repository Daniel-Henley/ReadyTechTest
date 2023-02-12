using Microsoft.AspNetCore.Mvc;
using ReadyTechTest.API.Shared;
using ReadyTechTest.API.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ReadyTechTest.API.Controllers;

[ApiController]
public class CoffeeController : ControllerBase
{
    private readonly ILogger<CoffeeController> _logger;
    //would store it in a db if it needs to be persisted, something like redis 
    private int count = 0;
    //would configure in appsettings if likely to change
    private const int maxCount = 5;

    public CoffeeController(ILogger<CoffeeController> logger)
    {
        _logger = logger;
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
    public ActionResult<CoffeeResponse> BrewCoffee()
    {
        //I'd generally use something like Mediatr to handle requests, but doesn't seem worthwhile for one endpoint
        var date = SystemTime.Now();
        if (date.Month == Constants.April && date.Day == Constants.First)
        {
            return new StatusCodeResult(StatusCodes.Status418ImATeapot);
        }

        count++;
        if (count == maxCount)
        {
            count = 0;
            return new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
        }

        return new CoffeeResponse(Constants.CoffeeResponseText, date);
    }
}
