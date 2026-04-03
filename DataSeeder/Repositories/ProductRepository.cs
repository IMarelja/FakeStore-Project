using DataSeeder.Data;
using FakeStore.View;

namespace DataSeeder.Repositories;

public class ProductRepository(PostgresDbContext ctx) : IProductRepository
{
    public async Task SeedAsync(IEnumerable<Product> products, IEnumerable<Review> reviews)
    {
        ctx.Products.AddRange(products);
        await ctx.SaveChangesAsync();

        var validUserIds = ctx.Users.Select(u => u.UserId).ToHashSet();
        ctx.Reviews.AddRange(reviews.Where(r => validUserIds.Contains(r.UserId)));
        await ctx.SaveChangesAsync();
    }
}
