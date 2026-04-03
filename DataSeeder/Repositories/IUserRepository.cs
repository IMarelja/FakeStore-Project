using FakeStore.View;

namespace DataSeeder.Repositories;

public interface IUserRepository
{
    Task SeedAsync(IEnumerable<User> users);
}
