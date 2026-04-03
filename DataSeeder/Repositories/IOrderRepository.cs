using FakeStore.View;

namespace DataSeeder.Repositories;

public interface IOrderRepository
{
    Task SeedAsync(IEnumerable<Order> orders, IEnumerable<(int OrderId, OrderItem Item)> items);
}
