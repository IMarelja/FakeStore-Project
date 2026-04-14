using FakeStore.WebApp.SoapClients;

namespace FakeStore.WebApp.Service;

public class SoapProductService : ISoapProductService
{
    public async Task<SearchResult> Quary(string term, double? minPrice, double? maxPrice)
    {

        using var client = new ProductSoapServiceClient();

        return await client.SearchProductsAsyncAsync(term, minPrice, maxPrice);
    }
}
