using FakeStore.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class WeatherController : Controller
{
    private readonly global::WebApp.gRPC.WeatherService.WeatherServiceClient _weatherClient;

    public WeatherController(global::WebApp.gRPC.WeatherService.WeatherServiceClient weatherClient)
    {
        _weatherClient = weatherClient;
    }

    public IActionResult Index()
    {
        return View(new WeatherViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Search(string city)
    {
        var vm = new WeatherViewModel { SearchCity = city, Searched = true };

        if (string.IsNullOrWhiteSpace(city))
            return View("Index", vm);

        var reply = await _weatherClient.GetWeatherAsync(new global::WebApp.gRPC.WeatherRequest { City = city });

        vm.Cities = reply.Items.Select(x => new WeatherCityViewModel
        {
            City = x.City,
            Temperature = x.Temperature,
            Humidity = x.Humidity,
            Pressure = x.Pressure,
            PressureTendency = x.PressureTendency,
            WindDirection = x.WindDirection,
            WindSpeed = x.WindSpeed,
            Description = x.Description,
            Date = x.Date,
            Term = x.Term
        }).ToList();
        vm.Found = vm.Cities.Count > 0;

        return View("Index", vm);
    }
}
