using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApplication2.Dao;

namespace WebApplication2.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class RoutingController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly IValidator<WeatherForecast> _validator;

    private readonly IOptionsSnapshot<ConfigDemo> _options;

    public RoutingController(
        IValidator<WeatherForecast> validator,
        IOptionsSnapshot<ConfigDemo> options)
    {
        _validator = validator;
        _options = options;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [HttpGet("/hello")]
    [HttpGet("test")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("getConfig")]
    public IActionResult GetConfig()
    {
        return Ok(_options.Value.WellcomeMessage);
    }
}
