using FakeStore.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class WeatherController : Controller
{
    private readonly FakeStore.gRPC.WeatherService.WeatherServiceClient _weatherClient;

    public WeatherController(FakeStore.gRPC.WeatherService.WeatherServiceClient weatherClient)
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

        var reply = await _weatherClient.GetWeatherAsync(new FakeStore.gRPC.WeatherRequest { City = city });

        vm.Found = reply.Found;
        if (reply.Found)
        {
            vm.City = reply.City;
            vm.Temperature = reply.Temperature;
            vm.Humidity = reply.Humidity;
            vm.Pressure = reply.Pressure;
            vm.PressureTendency = reply.PressureTendency;
            vm.WindDirection = reply.WindDirection;
            vm.WindSpeed = reply.WindSpeed;
            vm.Description = reply.Description;
            vm.Date = reply.Date;
            vm.Term = reply.Term;
        }

        return View("Index", vm);
    }
}
