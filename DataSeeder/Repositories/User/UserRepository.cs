using DataSeeder.Data;
using FakeStore.Models;

namespace DataSeeder.Repositories;

public class UserRepository(PostgresDbContext ctx) : IUserRepository
{
    public async Task SeedAsync(IEnumerable<User> users)
    {
        ctx.Users.AddRange(users);
        await ctx.SaveChangesAsync();
    }
}
