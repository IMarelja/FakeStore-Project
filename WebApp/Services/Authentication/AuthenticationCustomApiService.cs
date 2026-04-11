using System.Net.Http.Json;
using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public class AuthenticationCustomApiService : IAuthenticationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthenticationCustomApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AuthenticationResponse> login(LoginRequest loginRequest)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("authentication/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Login failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        return result ?? throw new InvalidOperationException("Authentication API returned an empty login response.");
    }

    public async Task<AuthenticationResponse> register(RegisterRequest registerRequest)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.PostAsJsonAsync("authentication/register", registerRequest);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Registration failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        return result ?? throw new InvalidOperationException("Authentication API returned an empty registration response.");
    }
}
