using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using DataSeeder.Repositories;
using FakeStore.Models;
using FakeStore.ViewModel;
using Json.Schema;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class UserService(IHttpClientFactory httpFactory, IUserRepository repo, IConfiguration config) : IUserService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var raw = await http.GetStringAsync($"{baseUrl}users");

        var schemaPath = Path.Combine(AppContext.BaseDirectory, "JsonSchemas", "User.json");
        var schema = JsonSchema.FromText(await File.ReadAllTextAsync(schemaPath));
        var result = schema.Evaluate(JsonNode.Parse(raw), new EvaluationOptions { OutputFormat = OutputFormat.List });

        if (!result.IsValid)
        {
            var errors = result.Details
                .Where(d => !d.IsValid && d.Errors is not null)
                .SelectMany(d => d.Errors!.Select(e => $"  {d.InstanceLocation}: {e.Value}"));

            throw new InvalidDataException($"User API response failed schema validation:\n{string.Join("\n", errors)}");
        }

        var apiUsers = JsonSerializer.Deserialize<List<UserRead>>(raw, _jsonOptions) ?? [];

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
