using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public class OrderPublicApiService : IOrderService
{

    private readonly IHttpClientFactory _httpClientFactory;

    public OrderPublicApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }


    public async Task<IEnumerable<OrderRead>> GetAll()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync("orders");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get orders failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<OrderRead>>();
        return result ?? [];
    }

    public Task<OrderCreateResponse> AddOrder(OrderCreate orderCreate)
    {
        throw new NotImplementedException();
    }

    public Task<OrderCreateResponse> EditOrder(string status)
    {
        throw new NotImplementedException();
    }

    public Task<OrderCreateResponse> EditOrder(int id, string status)
    {
        throw new NotImplementedException();
    }

    public Task<OrderRead?> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderRead>> GetOwn()
    {
        throw new NotImplementedException();
    }
}