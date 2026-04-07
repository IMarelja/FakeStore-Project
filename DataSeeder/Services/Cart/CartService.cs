using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Repositories;
using FakeStore.Models;
using FakeStore.ViewModel;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class CartService(IHttpClientFactory httpFactory, ICartRepository repo, IConfiguration config) : ICartService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var apiCarts = await http.GetFromJsonAsync<List<CartRead>>($"{baseUrl}carts", _jsonOptions) ?? [];

        var carts = apiCarts.Select(c => new Cart
        {
            CartId = c.cart_id,
            UserId = c.user_id
        });

        var items = apiCarts.SelectMany(c => c.items.Select(i => new CartItem
        {
            CartId    = c.cart_id,
            ProductId = i.product_id,
            Quantity  = i.quantity
        }));

        await repo.SeedAsync(carts, items);
    }
}
