using System.Text;
using System.Text.Json;
using FakeStore.Models;
using MyRestApi.DTO.User;
using MyRestApi.Middleware;

namespace MyRestApi.Repositories;

public class UserGraphQLRepo : IUserRepo
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions _writeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions _readOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public UserGraphQLRepo(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("graphql");
    }

    public async Task<List<User>> GetAllAsync()
    {
        const string query = """
            {
              users {
                userId
                username
                email
                password
                role
              }
            }
            """;

        var data = await SendAsync(query);
        return JsonSerializer.Deserialize<List<User>>(
            data.GetProperty("users").GetRawText(), _readOptions)!;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        const string query = """
            query($id: Int!) {
              user(id: $id) {
                userId
                username
                email
                password
                role
              }
            }
            """;

        var data = await SendAsync(query, new { id = userId });
        var userEl = data.GetProperty("user");

        if (userEl.ValueKind == JsonValueKind.Null)
            return null;

        return JsonSerializer.Deserialize<User>(userEl.GetRawText(), _readOptions)!;
    }

    public async Task<User?> UpdateAsync(int userId, UserUpdateDto req)
    {
        const string mutation = """
            mutation($id: Int!, $input: UserInput!) {
              updateUser(id: $id, input: $input) {
                userId
                username
                email
                password
                role
              }
            }
            """;

        var data = await SendAsync(mutation, new
        {
            id = userId,
            input = new
            {
                req.Username,
                req.Email,
                req.Password
            }
        });
        return JsonSerializer.Deserialize<User>(
            data.GetProperty("updateUser").GetRawText(), _readOptions)!;
    }

    public async Task<bool> DeleteAsync(int userId)
    {
        const string query = """
            mutation($id: Int!) {
              deleteUser(id: $id)
            }
            """;

        var data = await SendAsync(query, new { id = userId });
        return data.GetProperty("deleteUser").GetBoolean();
    }

    private async Task<JsonElement> SendAsync(string query, object? variables = null)
    {
        try
        {
            var body = JsonSerializer.Serialize(new { query, variables }, _writeOptions);
            var response = await _http.PostAsync("graphql", new StringContent(body, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(json).RootElement.GetProperty("data").Clone();
        }
        catch (HttpRequestException ex)
        {
            throw new GraphQLServiceException("GraphQL service is unavailable.", ex);
        }
    }
}
