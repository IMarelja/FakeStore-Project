using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MySoap.Models;

namespace MySoap.Services;

public class ProductSoapService(IWebHostEnvironment env) : IProductSoapService
{
    private string XmlPath => Path.Combine(env.ContentRootPath, "XML", "products.xml");
    private string XsdPath => Path.Combine(env.ContentRootPath, "Schemas", "products.xsd");

    public Task<SearchResult> SearchProductsAsync(string? term, double? minPrice, double? maxPrice)
    {
        var allProducts = ReadProductsFromXml();
        var normalizedTerm = term?.Trim();

        var filteredProducts = allProducts.Where(p =>
            (string.IsNullOrWhiteSpace(normalizedTerm)
             || p.name.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase)
             || p.description.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase)
             || p.category.Contains(normalizedTerm, StringComparison.OrdinalIgnoreCase))
            && (!minPrice.HasValue || p.price >= minPrice.Value)
            && (!maxPrice.HasValue || p.price <= maxPrice.Value));

        var products = filteredProducts
            .Select(p => new ProductSoap
            {
                Id = p.product_id,
                Name = p.name,
                Description = p.description,
                Price = Convert.ToDecimal(p.price),
                Category = p.category,
                Brand = p.brand,
                Rating = p.rating
            })
            .ToList();

        var result = new SearchResult
        {
            Products = products,
            Count = products.Count
        };

        return Task.FromResult(result);
    }

    private List<ReadProductXml> ReadProductsFromXml()
    {
        if (!File.Exists(XmlPath))
            return [];

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema
        };

        settings.Schemas.Add(null, XsdPath);

        var validationErrors = new List<string>();
        settings.ValidationEventHandler += (_, args) => validationErrors.Add(args.Message);

        using var reader = XmlReader.Create(XmlPath, settings);
        var serializer = new XmlSerializer(typeof(ProductsXml));

        var payload = serializer.Deserialize(reader) as ProductsXml;

        if (validationErrors.Count > 0)
            throw new InvalidDataException($"Invalid XML file: {string.Join(" | ", validationErrors)}");

        return payload?.Products ?? [];
    }
}
