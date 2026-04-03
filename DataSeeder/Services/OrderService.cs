using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Models;
using DataSeeder.Repositories;
using FakeStore.View;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class OrderService(IHttpClientFactory httpFactory, IOrderRepository repo, IConfiguration config) : IOrderService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var apiOrders = await http.GetFromJsonAsync<List<OrderApiModel>>($"{baseUrl}orders", _jsonOptions) ?? [];

        var orders = apiOrders.Select(o => new Order
        {
            OrderId     = o.OrderId,
            UserId      = o.UserId,
            OrderStatus = o.Status,
            TotalPrice  = o.TotalPrice
        });

        var items = apiOrders.SelectMany(o => o.Items.Select(i =>
            (o.OrderId, new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity })
        ));

        await repo.SeedAsync(orders, items);
    }
}
