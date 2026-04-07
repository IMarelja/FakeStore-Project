using FakeStore.Models;

namespace DataSeeder.Repositories;

public interface ICartRepository
{
    Task SeedAsync(IEnumerable<Cart> carts, IEnumerable<CartItem> items);
}
