using System.Text.Json;
using System.Text.Json.Nodes;
using DataSeeder.Repositories;
using FakeStore.Models;
using FakeStore.ViewModel;
using Json.Schema;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class CartService(IHttpClientFactory httpFactory, ICartRepository repo, IConfiguration config) : ICartService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var raw = await http.GetStringAsync($"{baseUrl}carts");

        var schemaPath = Path.Combine(AppContext.BaseDirectory, "JsonSchemas", "Cart.json");
        var schema = JsonSchema.FromText(await File.ReadAllTextAsync(schemaPath));
        var result = schema.Evaluate(JsonNode.Parse(raw), new EvaluationOptions { OutputFormat = OutputFormat.List });

        if (!result.IsValid)
        {
            var errors = result.Details
                .Where(d => !d.IsValid && d.Errors is not null)
                .SelectMany(d => d.Errors!.Select(e => $"  {d.InstanceLocation}: {e.Value}"));

            throw new InvalidDataException($"Cart API response failed schema validation:\n{string.Join("\n", errors)}");
        }

        var apiCarts = JsonSerializer.Deserialize<List<CartRead>>(raw, _jsonOptions) ?? [];

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
