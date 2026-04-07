using DataSeeder.Data;
using FakeStore.Models;

namespace DataSeeder.Repositories;

public class CartRepository(PostgresDbContext ctx) : ICartRepository
{
    public async Task SeedAsync(IEnumerable<Cart> carts, IEnumerable<CartItem> items)
    {
        ctx.Carts.AddRange(carts);
        await ctx.SaveChangesAsync();

        ctx.CartItems.AddRange(items);
        await ctx.SaveChangesAsync();
    }
}
