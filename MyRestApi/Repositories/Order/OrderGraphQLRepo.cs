using System.Text;
using System.Text.Json;
using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.Middleware;

namespace MyRestApi.Repositories;

public class OrderGraphQLRepo(IHttpClientFactory factory) : IOrderRepo
{
    private readonly HttpClient _http = factory.CreateClient("graphql");

    private static readonly JsonSerializerOptions _writeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions _readOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<Order>> GetAllAsync()
    {
        const string query = """
            {
              orders {
                orderId
                userId
                orderStatus
                totalPrice
                items { productId quantity }
              }
            }
            """;

        var data = await SendAsync(query);
        return JsonSerializer.Deserialize<List<Order>>(
            data.GetProperty("orders").GetRawText(), _readOptions)!;
    }

    public async Task<Order?> GetByIdAsync(int orderId)
    {
        const string query = """
            query($id: Int!) {
              order(id: $id) {
                orderId
                userId
                orderStatus
                totalPrice
                items { productId quantity }
              }
            }
            """;

        var data = await SendAsync(query, new { id = orderId });
        var el = data.GetProperty("order");
        return el.ValueKind == JsonValueKind.Null
            ? null
            : JsonSerializer.Deserialize<Order>(el.GetRawText(), _readOptions);
    }

    public async Task<Order> CreateAsync(OrderCreate req)
    {
        const string createOrderMutation = """
            mutation($input: OrderInput!) {
              createOrder(input: $input) {
                orderId
                userId
                orderStatus
                totalPrice
                items { productId quantity }
              }
            }
            """;

        var data = await SendAsync(createOrderMutation, new
        {
            input = new
            {
                userId      = req.UserId,
                orderStatus = "Pending",
                totalPrice  = 0.0
            }
        });

        var order = JsonSerializer.Deserialize<Order>(
            data.GetProperty("createOrder").GetRawText(), _readOptions)!;

        foreach (var item in req.Items)
            await CreateOrderItemAsync(order.OrderId, item.product_id, item.quantity);

        return (await GetByIdAsync(order.OrderId))!;
    }

    public async Task<Order?> UpdateAsync(int orderId, OrderUpdate req)
    {
        const string mutation = """
            mutation($id: Int!, $input: OrderUpdateInput!) {
              updateOrder(id: $id, input: $input) {
                orderId
                userId
                orderStatus
                totalPrice
                items { productId quantity }
              }
            }
            """;

        var data = await SendAsync(mutation, new
        {
            id    = orderId,
            input = new { orderStatus = req.Status }
        });

        var el = data.GetProperty("updateOrder");
        return el.ValueKind == JsonValueKind.Null
            ? null
            : JsonSerializer.Deserialize<Order>(el.GetRawText(), _readOptions);
    }

    public async Task<bool> DeleteAsync(int orderId)
    {
        const string mutation = """
            mutation($id: Int!) {
              deleteOrder(id: $id)
            }
            """;

        var data = await SendAsync(mutation, new { id = orderId });
        return data.GetProperty("deleteOrder").GetBoolean();
    }

    private async Task CreateOrderItemAsync(int orderId, int productId, int quantity)
    {
        const string mutation = """
            mutation($input: OrderItemInput!) {
              createOrderItem(input: $input) {
                productId
                quantity
              }
            }
            """;

        await SendAsync(mutation, new
        {
            input = new { orderId, productId, quantity }
        });
    }

    private async Task DeleteOrderItemAsync(int orderId, int productId)
    {
        const string mutation = """
            mutation($orderId: Int!, $productId: Int!) {
              deleteOrderItem(orderId: $orderId, productId: $productId)
            }
            """;

        await SendAsync(mutation, new { orderId, productId });
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
