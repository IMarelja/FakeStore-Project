using DataSeeder.Data;
using FakeStore.View;

namespace DataSeeder.Repositories;

public class ProductRepository(PostgresDbContext ctx) : IProductRepository
{
    public async Task SeedAsync(IEnumerable<Product> products, IEnumerable<Review> reviews)
    {
        ctx.Products.AddRange(products);
        await ctx.SaveChangesAsync();

        ctx.Reviews.AddRange(reviews);
        await ctx.SaveChangesAsync();
    }
}
