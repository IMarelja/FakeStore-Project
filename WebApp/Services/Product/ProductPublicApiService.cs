using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public class ProductPublicApiService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductPublicApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<ProductRead>> GetAll()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync("products");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get products failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ProductRead>>();
        return result ?? [];
    }
    public Task<ReviewProductRead> AddReview(int productId, ReviewProductCreate review)
    {
        throw new NotImplementedException();
    }

    public Task<ProductRead> CreateProduct(ProductCreate product)
    {
        throw new NotImplementedException();
    }

    public Task<ProductRead?> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ProductRead?> UpdateProduct(int id, ProductUpdate product)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteProduct(int id)
    {
        throw new NotImplementedException();
    }
}
