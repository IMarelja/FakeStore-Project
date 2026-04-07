using DataSeeder.Data;
using FakeStore.Models;

namespace DataSeeder.Repositories;

public class OrderRepository(PostgresDbContext ctx) : IOrderRepository
{
    public async Task SeedAsync(IEnumerable<Order> orders, IEnumerable<(int OrderId, OrderItem Item)> items)
    {
        ctx.Orders.AddRange(orders);
        await ctx.SaveChangesAsync();

        foreach (var (orderId, item) in items)
        {
            ctx.OrderItems.Add(item);
            ctx.Entry(item).Property("OrderId").CurrentValue = orderId;
        }
        await ctx.SaveChangesAsync();
    }
}
