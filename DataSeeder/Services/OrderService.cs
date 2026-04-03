using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Data;
using DataSeeder.Models;
using DataSeeder.Repositories;
using FakeStore.View;
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

        var apiOrders = await http.GetFromJsonAsync<List<OrderApiModel>>($"{baseUrl}orders", _jsonOptions) ?? [];

        var validUserIds    = (await ctx.Users.Select(u => u.UserId).ToListAsync()).ToHashSet();
        var validProductIds = (await ctx.Products.Select(p => p.ProductId).ToListAsync()).ToHashSet();

        var orders = apiOrders
            .Where(o => validUserIds.Contains(o.UserId))
            .Select(o => new Order
            {
                OrderId     = o.OrderId,
                UserId      = o.UserId,
                OrderStatus = o.Status,
                TotalPrice  = o.TotalPrice
            });

        var validOrderIds = apiOrders.Where(o => validUserIds.Contains(o.UserId)).Select(o => o.OrderId).ToHashSet();

        var items = apiOrders
            .Where(o => validOrderIds.Contains(o.OrderId))
            .SelectMany(o => o.Items
                .Where(i => validProductIds.Contains(i.ProductId))
                .Select(i => (o.OrderId, new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity })
            ));

        await repo.SeedAsync(orders, items);
    }
}
