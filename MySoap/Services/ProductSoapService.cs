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

    private async Task FetchAndBuildXmlAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var raw = await http.GetStringAsync($"{baseUrl}products");
        var apiProducts = JsonSerializer.Deserialize<List<ProductRead>>(raw, _jsonOptions) ?? [];

        var products = apiProducts.Select(p => new Product
        {
            ProductId   = p.product_id,
            Name        = p.name,
            Description = p.description,
            Price       = (decimal)p.price,
            Unit        = p.unit,
            Image       = p.image,
            Discount    = p.discount,
            Available   = p.availability,
            Brand       = p.brand,
            Category    = p.category,
            Rating      = p.rating
        }).ToList();

        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Products",
                products.Select(p => new XElement("Product",
                    new XElement("Id",          p.ProductId),
                    new XElement("Name",        p.Name),
                    new XElement("Description", p.Description),
                    new XElement("Price",       p.Price),
                    new XElement("Category",    p.Category),
                    new XElement("Brand",       p.Brand),
                    new XElement("Rating",      p.Rating)
                ))
            )
        );

        doc.Save(XmlPath);
    }

    public async Task<SearchResult> SearchProductsAsync(string term)
    {
        await FetchAndBuildXmlAsync();

        var xpathDoc = new XPathDocument(XmlPath);
        var nav      = xpathDoc.CreateNavigator();
        var lower    = term.ToLowerInvariant();

        var expr = nav.Compile(
            $"//Product[contains(translate(Name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'{lower}') " +
            $"or contains(translate(Category,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'{lower}')]");

        var nodes   = nav.Select(expr);
        var results = new List<ProductSoap>();

        while (nodes.MoveNext())
        {
            var n = nodes.Current!;
            results.Add(new ProductSoap
            {
                Id          = int.Parse(n.SelectSingleNode("Id")!.Value),
                Name        = n.SelectSingleNode("Name")!.Value,
                Description = n.SelectSingleNode("Description")!.Value,
                Price       = decimal.Parse(n.SelectSingleNode("Price")!.Value, System.Globalization.CultureInfo.InvariantCulture),
                Category    = n.SelectSingleNode("Category")!.Value,
                Brand       = n.SelectSingleNode("Brand")!.Value,
                Rating      = double.Parse(n.SelectSingleNode("Rating")!.Value, System.Globalization.CultureInfo.InvariantCulture)
            });
        }

        return new SearchResult { Products = results, Count = results.Count };
    }

    public async Task<ValidationResult> ValidateProductsAsync()
    {
        if (!File.Exists(XmlPath))
            await FetchAndBuildXmlAsync();

        var messages = new List<string>();
        var schemas  = new XmlSchemaSet();
        schemas.Add(null, XsdPath);

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas        = schemas
        };
        settings.ValidationEventHandler += (_, e) => messages.Add(e.Message);

        using var reader = XmlReader.Create(XmlPath, settings);
        while (reader.Read()) { }

        return new ValidationResult { IsValid = messages.Count == 0, Messages = messages };
    }
}
