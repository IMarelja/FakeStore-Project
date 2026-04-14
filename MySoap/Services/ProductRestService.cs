using System.Net.Http.Json;
using FakeStore.ViewModel;
using System.Xml.Serialization;
using System.Linq;
using System.Xml.Linq;
using MySoap.Models;

namespace MySoap.Services;

public class ProductRestService : IProductRestService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;

    public ProductRestService(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpFactory = httpFactory;
        _config = config;
    }

    public async Task<List<ReadProductXml>> GetAll()
    {
        using var http = _httpFactory.CreateClient();

        var baseUrl = _config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException(
                "Missing API base URL. Configure Api:BaseUrl or ConnectionStrings:DefaultConnection.");

        var response = await http.GetAsync($"{baseUrl}/products");

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

    public Task ToXmlFile(List<ReadProductXml> products)
    {
        throw new NotImplementedException();
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
        List<ReadProductXml> productsXml = new List<ReadProductXml>();

        foreach(var product in products)
        {
            ReadProductXml productXml = new ReadProductXml
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

            productsXml.Add(productXml);
        }

        return productsXml;
    }
}
