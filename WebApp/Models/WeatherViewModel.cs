namespace FakeStore.WebApp.Models;

public class WeatherViewModel
{
    public string? SearchCity { get; set; }
    public bool Searched { get; set; }
    public bool Found { get; set; }
    public List<WeatherCityViewModel> Cities { get; set; } = [];
}

public class WeatherCityViewModel
{
    public string City { get; set; } = "";
    public string Temperature { get; set; } = "";
    public string Humidity { get; set; } = "";
    public string Pressure { get; set; } = "";
    public string PressureTendency { get; set; } = "";
    public string WindDirection { get; set; } = "";
    public string WindSpeed { get; set; } = "";
    public string Description { get; set; } = "";
    public string Date { get; set; } = "";
    public string Term { get; set; } = "";
}
