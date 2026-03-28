using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FakeStore.View;
using FakeStore.ViewModel;
using Microsoft.IdentityModel.Tokens;

namespace MyRestApi.Repositories;

public class AuthenticationRepo : IAuthenticationRepo
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    private static readonly JsonSerializerOptions _writeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions _readOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthenticationRepo(IHttpClientFactory factory, IConfiguration config)
    {
        _http = factory.CreateClient("graphql");
        _config = config;
    }

    private async Task<JsonElement> SendAsync(string query, object? variables = null)
    {
        var body = JsonSerializer.Serialize(new { query, variables }, _writeOptions);
        var response = await _http.PostAsync("graphql", new StringContent(body, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement.GetProperty("data").Clone();
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest req)
    {
        const string query = """
            {
              users {
                userId
                username
                email
                password
              }
            }
            """;

        var data = await SendAsync(query);
        var users = JsonSerializer.Deserialize<List<User>>(
            data.GetProperty("users").GetRawText(), _readOptions)!;

        var user = users.FirstOrDefault(u =>
            u.Username == req.username && u.Password == req.password);

        if (user is null)
            return null;

        return GenerateToken(user, req.remember_me);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest req)
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
        var user = JsonSerializer.Deserialize<User>(
            data.GetProperty("createUser").GetRawText(), _readOptions)!;

        return GenerateToken(user, req.remember_me);
    }

    private LoginResponse GenerateToken(User user, bool rememberMe)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.Add(rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromHours(1));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            ],
            expires: expiry,
            signingCredentials: creds
        );

        return new LoginResponse
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expires_at = expiry
        };
    }
}
