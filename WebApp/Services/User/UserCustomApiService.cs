using FakeStore.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FakeStore.WebApp.Service;

public class UserCustomApiService : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJwtService _jwtService;

    public UserCustomApiService(IHttpClientFactory httpClientFactory, IJwtService jwtService)
    {
        _httpClientFactory = httpClientFactory;
        _jwtService = jwtService;
    }

    public async Task<bool> DeleteMe()
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync("user/me");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Delete profile failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
    }

    public async Task<bool> DeleteUser(int id)
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync($"user/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        if (response.IsSuccessStatusCode)
            return true;

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Delete user failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
    }

    public async Task<UserRead?> EditMe(UserUpdate user)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync("user/me", user);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Update profile failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<UserRead>();
    }

    public async Task<UserRead?> EditUser(int id, UserUpdate user)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync($"user/{id}", user);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Update user failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<UserRead>();
    }

    public async Task<IEnumerable<UserRead>> GetAll()
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("user");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get users failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<UserRead>>();
        return result ?? [];
    }

    public async Task<UserRead?> GetById(int id)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"user/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get user failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<UserRead>();
    }

    public async Task<UserRead?> GetMe()
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("user/me");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get profile failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<UserRead>();
    }

    private HttpClient CreateAuthorizedClient()
    {
        var token = _jwtService.GetAccessToken();
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("No access token found for current user session.");

        var client = _httpClientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return client;
    }
}
