using FakeStore.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FakeStore.WebApp.Service;

public class CartCustomApiService : ICartService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJwtService _jwtService;

    public CartCustomApiService(IHttpClientFactory httpClientFactory, IJwtService jwtService)
    {
        _httpClientFactory = httpClientFactory;
        _jwtService = jwtService;
    }

    public async Task<CartRead> addItemToOwnCart(CartItemAdd item)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync("cart/me/item", item);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Add item to cart failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<CartRead>()
            ?? throw new InvalidOperationException("Cart API returned an empty cart response.");
    }

    public async Task<bool> deleteItemFromOwnCart(int productId)
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync($"cart/me/item?product_id={productId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Delete item from cart failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
    }

    public async Task<bool> deleteOwnCart()
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync("cart/me");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Delete own cart failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
    }

    public async Task<bool> deleteCart(int cartId)
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync($"cart/{cartId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Delete cart failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
    }

    public async Task<CartRead?> editItemToOwnCart(int cartItemId, int quantity)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"cart/me/{cartItemId}/item", new CartItemEdit
        {
            Quantity = quantity
        });

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Edit cart item failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<CartRead>();
    }

    public async Task<IEnumerable<CartRead>> getAll()
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("cart");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get carts failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<CartRead>>();
        return result ?? [];
    }

    public async Task<CartRead?> getById(int id)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"cart/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get cart failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<CartRead>();
    }

    public async Task<CartRead?> getOwnCart()
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("cart/me");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get own cart failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<CartRead>();
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
