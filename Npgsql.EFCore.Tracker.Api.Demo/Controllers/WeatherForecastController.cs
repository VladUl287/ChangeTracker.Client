using Microsoft.AspNetCore.Mvc;
using Npgsql.EFCore.Tracker.AspNet.Attributes;

namespace Npgsql.EFCore.Tracker.Api.Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet(Name = "GetWeatherForecast")]
    [Track("weatherforecast", "Tracker")]
    public IEnumerable<WeatherForecast> Get()
    {
        return [.. Enumerable.Range(1, 1000).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })];
    }
}
