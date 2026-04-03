using System.Text;
using System.Text.Json;
using FakeStore.View;
using FakeStore.ViewModel;
using MyRestApi.Middleware;

namespace MyRestApi.Repositories;

public class AuthenticationGraphQLRepo : IAuthenticationRepo
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

    public AuthenticationGraphQLRepo(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("graphql");
    }

    public async Task<User?> UserByEmailAsync(string email)
    {
        const string query = """
            query($email: String!) {
              userByEmail(email: $email) {
                userId
                username
                email
                password
              }
            }
            """;

        var data = await SendAsync(query, new { email });
        var el = data.GetProperty("userByEmail");
        return el.ValueKind == JsonValueKind.Null
            ? null
            : JsonSerializer.Deserialize<User>(el.GetRawText(), _readOptions)!;
    }

    public async Task<User?> UserByUsernameAsync(string username)
    {
        const string query = """
            query($username: String!) {
              userByUsername(username: $username) {
                userId
                username
                email
                password
              }
            }
            """;

        var data = await SendAsync(query, new { username });
        var el = data.GetProperty("userByUsername");
        return el.ValueKind == JsonValueKind.Null
            ? null
            : JsonSerializer.Deserialize<User>(el.GetRawText(), _readOptions)!;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        var user = await UserByUsernameAsync(username);
        return user is not null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await UserByEmailAsync(email);
        return user is not null;
    }

    public async Task<User> CreateUserAsync(RegisterRequest req)
    {
        const string mutation = """
            mutation($input: UserInput!) {
              createUser(input: $input) {
                userId
                username
                email
                password
              }
            }
            """;

        var data = await SendAsync(mutation, new { input = new { req.username, req.email, req.password } });
        return JsonSerializer.Deserialize<User>(
            data.GetProperty("createUser").GetRawText(), _readOptions)!;
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
