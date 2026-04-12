using FakeStore.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace FakeStore.WebApp.Service;

public class ProductCustomApiService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductCustomApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<ReviewProductRead> AddReview(ReviewProductCreate review)
    {
        throw new InvalidOperationException("Product id is required for custom API review creation.");
    }

    public async Task<ProductRead> CreateProduct(ProductCreate product)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("product", product);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Create product failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<ProductRead>()
            ?? throw new InvalidOperationException("Product API returned an empty create response.");
    }

    public async Task<IEnumerable<ProductRead>> GetAll()
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("product");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get products failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ProductRead>>();
        return result ?? [];
    }

    public async Task<ProductRead?> GetById(int id)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"product/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get product failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<ProductRead>();
    }

    public async Task<ProductRead?> UpdateProduct(int id, ProductUpdate product)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"product/{id}", product);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Update product failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<ProductRead>();
    }

    private HttpClient CreateAuthorizedClient()
    {
        var token = _httpContextAccessor.HttpContext?.User.FindFirstValue("access_token");
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("No access token found for current user session.");

        var client = _httpClientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
