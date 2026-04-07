using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Data;
using DataSeeder.Repositories;
using FakeStore.Models;
using FakeStore.ViewModel;
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

        var apiOrders = await http.GetFromJsonAsync<List<OrderRead>>($"{baseUrl}orders", _jsonOptions) ?? [];

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
