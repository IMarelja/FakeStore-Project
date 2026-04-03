using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Models;
using DataSeeder.Repositories;
using FakeStore.View;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class CartService(IHttpClientFactory httpFactory, ICartRepository repo, IConfiguration config) : ICartService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var apiCarts = await http.GetFromJsonAsync<List<CartApiModel>>($"{baseUrl}carts", _jsonOptions) ?? [];

        var carts = apiCarts.Select(c => new Cart
        {
            CartId = c.CartId,
            UserId = c.UserId
        });

        var items = apiCarts.SelectMany(c => c.Items.Select(i => new CartItem
        {
            CartId    = c.CartId,
            ProductId = i.ProductId,
            Quantity  = i.Quantity
        }));

        await repo.SeedAsync(carts, items);
    }
}
