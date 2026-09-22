using FakeStore.WebApp.SoapClients;

namespace FakeStore.WebApp.Service;

public class SoapProductService : ISoapProductService
{
    private readonly string _baseUrl;

    public SoapProductService(IConfiguration configuration)
    {
        _baseUrl = configuration["SoapService:BaseUrl"] ?? "http://localhost:5123/ProductService.asmx";
    }

    public async Task<SearchResult> Quary(string term, double? minPrice, double? maxPrice)
    {
        using var client = new ProductSoapServiceClient(
            ProductSoapServiceClient.EndpointConfiguration.BasicHttpBinding_IProductSoapService, _baseUrl);

        return await client.SearchProductsAsync(term, minPrice, maxPrice);
    }
}
