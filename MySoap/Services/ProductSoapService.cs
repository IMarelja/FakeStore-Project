using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using FakeStore.Models;
using FakeStore.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MySoap.Models;

namespace MySoap.Services;

public class ProductSoapService(
    IHttpClientFactory httpFactory,
    IConfiguration config,
    IWebHostEnvironment env) : IProductSoapService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    private string XmlPath => Path.Combine(env.ContentRootPath, "products.xml");
    private string XsdPath => Path.Combine(env.ContentRootPath, "Schemas", "products.xsd");

    public Task<SearchResult> SearchProductsAsync(string? term, double? minPrice, double? maxPrice)
    {
        throw new NotImplementedException();
    }
}
