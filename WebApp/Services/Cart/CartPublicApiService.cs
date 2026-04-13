using FakeStore.ViewModel;
using System.Net.Http.Json;

namespace FakeStore.WebApp.Service;

public class CartPublicApiService : ICartService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CartPublicApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private static NotSupportedException BuildNotSupported() =>
        new("Cart write operations are available only when using the Custom API mode.");

    public async Task<IEnumerable<CartRead>> getAll()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync("carts");

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
        var carts = await getAll();
        return carts.FirstOrDefault(cart => cart.cart_id == id);
    }

    public Task<CartRead?> getOwnCart()
    {
        return Task.FromResult<CartRead?>(null);
    }

    public Task<CartRead> addItemToOwnCart(CartItemAdd item)
    {
        throw BuildNotSupported();
    }

    public Task<CartRead?> editItemToOwnCart(int cartItemId, int quantity)
    {
        throw BuildNotSupported();
    }

    public Task<bool> deleteItemFromOwnCart(int productId)
    {
        throw BuildNotSupported();
    }

    public Task<bool> deleteOwnCart()
    {
        throw BuildNotSupported();
    }

    public Task<bool> deleteCart(int cartId)
    {
        throw BuildNotSupported();
    }
}
