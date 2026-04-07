using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Repositories;
using FakeStore.Models;
using FakeStore.ViewModel;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class UserService(IHttpClientFactory httpFactory, IUserRepository repo, IConfiguration config) : IUserService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var apiUsers = await http.GetFromJsonAsync<List<UserRead>>($"{baseUrl}users", _jsonOptions) ?? [];

        var users = apiUsers.Select(u => new User
        {
            UserId   = u.user_id,
            Username = u.username,
            Email    = u.email,
            Password = u.password,
            Role     = "read-only"
        });

        await repo.SeedAsync(users);
    }
}
