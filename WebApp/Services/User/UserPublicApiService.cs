using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public class UserPublicApiService : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UserPublicApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<UserRead>> GetAll()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
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

    public Task<bool> DeleteMe()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUser(int id)
    {
        throw new NotImplementedException();
    }

    public Task<UserRead?> EditMe(UserUpdate user)
    {
        throw new NotImplementedException();
    }

    public Task<UserRead?> EditUser(int id, UserUpdate user)
    {
        throw new NotImplementedException();
    }

    public Task<UserRead?> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<UserRead?> GetMe()
    {
        throw new NotImplementedException();
    }
}