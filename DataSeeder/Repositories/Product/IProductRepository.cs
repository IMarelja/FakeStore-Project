using FakeStore.Models;

namespace DataSeeder.Repositories;

public interface IProductRepository
{
    Task SeedAsync(IEnumerable<Product> products, IEnumerable<Review> reviews);
}
