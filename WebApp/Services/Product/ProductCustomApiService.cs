using FakeStore.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FakeStore.WebApp.Service;

public class ProductCustomApiService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJwtService _jwtService;

    public ProductCustomApiService(IHttpClientFactory httpClientFactory, IJwtService jwtService)
    {
        _httpClientFactory = httpClientFactory;
        _jwtService = jwtService;
    }

    public async Task<ReviewProductRead> AddReview(int productId, ReviewProductCreate review)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync($"product/{productId}/review", review);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Create review failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<ReviewProductRead>()
            ?? throw new InvalidOperationException("Product API returned an empty review response.");
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

    public async Task<bool> DeleteProduct(int id)
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync($"product/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Delete product failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
    }

    private HttpClient CreateAuthorizedClient()
    {
        var token = _jwtService.GetAccessToken();
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("No access token found for current user session.");

        var client = _httpClientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
