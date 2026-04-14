using FakeStore.ViewModel;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FakeStore.WebApp.Service;

public class OrderCustomApiService : IOrderService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IJwtService _jwtService;

    public OrderCustomApiService(IHttpClientFactory httpClientFactory, IJwtService jwtService)
    {
        _httpClientFactory = httpClientFactory;
        _jwtService = jwtService;
    }

    public async Task<OrderCreateResponse> AddOrder(OrderCreate orderCreate)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PutAsJsonAsync("orders", orderCreate);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Place order failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<OrderCreateResponse>()
            ?? throw new InvalidOperationException("Order API returned an empty create response.");
    }

    public async Task<OrderCreateResponse> EditOrder(int id, string status)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync($"orders/{id}", new OrderUpdate
        {
            Status = status
        });

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new HttpRequestException("Order not found.", null, response.StatusCode);
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Update order failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return await response.Content.ReadFromJsonAsync<OrderCreateResponse>()
            ?? throw new InvalidOperationException("Order API returned an empty update response.");
    }

    public async Task<bool> DeleteOrder(int id)
    {
        var client = CreateAuthorizedClient();
        var response = await client.DeleteAsync($"orders/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Delete order failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        return true;
    }

    public async Task<IEnumerable<OrderRead>> GetAll()
    {
        var client = CreateAuthorizedClient();
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

    public async Task<OrderRead?> GetById(int id)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync($"orders/status?order_id={id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get order status failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var status = await response.Content.ReadFromJsonAsync<OrderStatusRead>();
        if (status is null)
        {
            return null;
        }

        return new OrderRead
        {
            order_id = status.order_id,
            user_id = status.user_id,
            status = status.status,
            total_price = status.total_price,
            items = []
        };
    }

    public async Task<IEnumerable<OrderRead>> GetOwn()
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("orders/me");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get own orders failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<OrderRead>>();
        return result ?? [];
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
