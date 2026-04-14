using System.Net.Http.Json;
using FakeStore.ViewModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using MySoap.Models;
using Microsoft.AspNetCore.Hosting;

namespace MySoap.Services;

public class ProductRestService : IProductRestService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;

    public ProductRestService(
        IHttpClientFactory httpFactory,
        IConfiguration config,
        IWebHostEnvironment env)
    {
        _httpFactory = httpFactory;
        _config = config;
        _env = env;
    }

    public async Task<List<ReadProductXml>> GetAll()
    {
        using var http = _httpFactory.CreateClient();

        var baseUrl = _config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException(
                "Missing API base URL. Configure Api:BaseUrl or ConnectionStrings:DefaultConnection.");

        var response = await http.GetAsync($"{baseUrl}products");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get products failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var products = await response.Content.ReadFromJsonAsync<List<ProductRead>>();

        if(products == null || products.Count == 0)
            return new List<ReadProductXml>();

        return ToReadProductXml(products);
    }

    public async Task ToXmlFile(List<ReadProductXml> products)
    {
        ArgumentNullException.ThrowIfNull(products);

        var xmlDirectory = Path.Combine(_env.ContentRootPath, "XML");
        Directory.CreateDirectory(xmlDirectory);
        var xmlPath = Path.Combine(xmlDirectory, "products.xml");
        var productPayload = new ProductsXml { Products = products };


        var serializer = new XmlSerializer(typeof(ProductsXml));

        await using var stream = File.Create(xmlPath);
        var settings = new XmlWriterSettings{
            Async = true,
            Indent = true,
            Encoding = new UTF8Encoding(false)
        };

        await using var writer = XmlWriter.Create(stream, settings);

        serializer.Serialize(writer, productPayload);
        await writer.FlushAsync();
    }

    public Task<bool> VerifyXmlFile()
    {
        throw new NotImplementedException();
    }

    private static ReadProductXml ToReadProductXml(ProductRead product)
    {
        return new ReadProductXml
        {
            product_id = product.product_id,
            name = product.name,
            description = product.description,
            price = product.price,
            unit = product.unit,
            image = product.image,
            discount = product.discount,
            availability = product.availability,
            brand = product.brand,
            category = product.category,
            rating = product.rating
        };
    }

    private static List<ReadProductXml> ToReadProductXml(List<ProductRead> products)
    {
        return products.Select(ToReadProductXml).ToList();
    }
}
