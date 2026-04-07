using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using DataSeeder.Data;
using DataSeeder.Repositories;
using FakeStore.Models;
using FakeStore.ViewModel;
using Json.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class OrderService(IHttpClientFactory httpFactory, IOrderRepository repo, IConfiguration config, PostgresDbContext ctx) : IOrderService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var raw = await http.GetStringAsync($"{baseUrl}orders");

        var schemaPath = Path.Combine(AppContext.BaseDirectory, "JsonSchemas", "Order.json");
        var schema = JsonSchema.FromText(await File.ReadAllTextAsync(schemaPath));
        var result = schema.Evaluate(JsonNode.Parse(raw), new EvaluationOptions { OutputFormat = OutputFormat.List });

        if (!result.IsValid)
        {
            var errors = result.Details
                .Where(d => !d.IsValid && d.Errors is not null)
                .SelectMany(d => d.Errors!.Select(e => $"  {d.InstanceLocation}: {e.Value}"));

            throw new InvalidDataException($"Order API response failed schema validation:\n{string.Join("\n", errors)}");
        }

        var apiOrders = JsonSerializer.Deserialize<List<OrderRead>>(raw, _jsonOptions) ?? [];

        var validUserIds    = (await ctx.Users.Select(u => u.UserId).ToListAsync()).ToHashSet();
        var validProductIds = (await ctx.Products.Select(p => p.ProductId).ToListAsync()).ToHashSet();

        var orders = apiOrders
            .Where(o => validUserIds.Contains(o.user_id))
            .Select(o => new Order
            {
                OrderId     = o.order_id,
                UserId      = o.user_id,
                OrderStatus = o.status,
                TotalPrice  = (decimal)o.total_price
            });

        var validOrderIds = apiOrders.Where(o => validUserIds.Contains(o.user_id)).Select(o => o.order_id).ToHashSet();

        var items = apiOrders
            .Where(o => validOrderIds.Contains(o.order_id))
            .SelectMany(o => o.items
                .Where(i => validProductIds.Contains(i.product_id))
                .Select(i => (o.order_id, new OrderItem { ProductId = i.product_id, Quantity = i.quantity })
            ));

        await repo.SeedAsync(orders, items);
    }
}
