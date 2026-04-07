using FakeStore.Models;

namespace DataSeeder.Repositories;

public interface IUserRepository
{
    Task SeedAsync(IEnumerable<User> users);
}
