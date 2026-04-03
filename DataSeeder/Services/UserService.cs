using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Models;
using DataSeeder.Repositories;
using FakeStore.View;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class UserService(IHttpClientFactory httpFactory, IUserRepository repo, IConfiguration config) : IUserService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var apiUsers = await http.GetFromJsonAsync<List<UserApiModel>>($"{baseUrl}users", _jsonOptions) ?? [];

        var users = apiUsers.Select(u => new User
        {
            UserId   = u.UserId,
            Username = u.Username,
            Email    = u.Email,
            Password = u.Password,
            Role     = u.Role
        });

        await repo.SeedAsync(users);
    }
}
