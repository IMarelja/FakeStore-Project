using FakeStore.Models;

namespace DataSeeder.Repositories;

public interface IOrderRepository
{
    Task SeedAsync(IEnumerable<Order> orders, IEnumerable<(int OrderId, OrderItem Item)> items);
}
