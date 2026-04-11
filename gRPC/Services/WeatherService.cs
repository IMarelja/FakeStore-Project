using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;

namespace FakeStore.gRPC.Services;

public class WeatherService : global::FakeStore.gRPC.WeatherService.WeatherServiceBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly string _urlConnection;

    public WeatherService(IHttpClientFactory httpFactory, IConfiguration configuration)
    {
        _httpFactory = httpFactory;
        _urlConnection = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    }

    public override async Task<WeatherListReply> GetWeather(WeatherRequest request, ServerCallContext context)
    {
        var http = _httpFactory.CreateClient();
        var xml = await http.GetStringAsync(_urlConnection);
        var doc = XDocument.Parse(xml);

        var dateEl = doc.Root?.Element("DatumTermin");
        var date = dateEl?.Element("Datum")?.Value ?? "";
        var term = dateEl?.Element("Termin")?.Value ?? "";

        var matches = doc.Root?.Elements("Grad")
            .Where(g =>
                g.Element("GradIme")?.Value
                    .Contains(request.City, StringComparison.OrdinalIgnoreCase) == true)
            .ToList()
            ?? [];

        var response = new WeatherListReply();

        foreach (var grad in matches)
        {
            var p = grad.Element("Podatci");
            response.Items.Add(new WeatherReply
            {
                Found = true,
                City = grad.Element("GradIme")?.Value ?? "",
                Temperature = p?.Element("Temp")?.Value ?? "",
                Humidity = p?.Element("Vlaga")?.Value ?? "",
                Pressure = p?.Element("TlakMBara")?.Value ?? "",
                PressureTendency = p?.Element("TlakTend")?.Value ?? "",
                WindDirection = p?.Element("VjetarSmjer")?.Value ?? "",
                WindSpeed = p?.Element("VjetarBrzina")?.Value ?? "",
                Description = p?.Element("Vrijemee")?.Value ?? "",
                Date = date,
                Term = term,
            });
        }

        return response;
    }
}
