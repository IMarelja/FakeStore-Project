using System.Text;
using System.Text.Json;
using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.Middleware;

namespace MyRestApi.Repositories;

public class CartGraphQLRepo(IHttpClientFactory factory) : ICartRepo
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

    public async Task<List<Cart>> GetAllAsync()
    {
        const string query = """
            {
              carts {
                cartId
                userId
                items { cartItemId cartId productId quantity }
              }
            }
            """;

        var data = await SendAsync(query);
        return JsonSerializer.Deserialize<List<Cart>>(
            data.GetProperty("carts").GetRawText(), _readOptions)!;
    }

    public async Task<Cart?> GetByIdAsync(int cartId)
    {
        const string query = """
            query($id: Int!) {
              cart(id: $id) {
                cartId
                userId
                items { cartItemId cartId productId quantity }
              }
            }
            """;

        var data = await SendAsync(query, new { id = cartId });
        var el = data.GetProperty("cart");
        return el.ValueKind == JsonValueKind.Null
            ? null
            : JsonSerializer.Deserialize<Cart>(el.GetRawText(), _readOptions);
    }

    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        const string query = """
            query($userId: Int!) {
              cartByUser(userId: $userId) {
                cartId
                userId
                items { cartItemId cartId productId quantity }
              }
            }
            """;

        var data = await SendAsync(query, new { userId });
        var el = data.GetProperty("cartByUser");

        if (el.ValueKind != JsonValueKind.Null)
            return JsonSerializer.Deserialize<Cart>(el.GetRawText(), _readOptions);

        return await AddUserCartAsync(userId);
    }

    public async Task<Cart?> AddUserCartAsync(int userId)
    {
        const string mutation = """
            mutation($input: CartInput!) {
              createCart(input: $input) {
                cartId
                userId
                items { cartItemId cartId productId quantity }
              }
            }
            """;

        var data = await SendAsync(mutation, new { input = new { userId } });
        return JsonSerializer.Deserialize<Cart>(
            data.GetProperty("createCart").GetRawText(), _readOptions);
    }

    public async Task<Cart?> AddItemAsync(int cartId, CartItemAdd req)
    {
        const string mutation = """
            mutation($input: CartItemInput!) {
              createCartItem(input: $input) {
                cartItemId
                cartId
                productId
                quantity
              }
            }
            """;

        var data = await SendAsync(mutation, new
        {
            input = new
            {
                cartId,
                productId = req.ProductId,
                quantity  = req.Quantity
            }
        });

        var itemEl = data.GetProperty("createCartItem");
        if (itemEl.ValueKind == JsonValueKind.Null)
            return null;

        return await GetByIdAsync(cartId);
    }

    public async Task<Cart?> EditItemAsync(int itemId, CartItemEdit req)
    {
        const string mutation = """
            mutation($input: CartItemUpdateInput!) {
              updateCartItem(input: $input) {
                cartItemId
                cartId
                productId
                quantity
              }
            }
            """;

        var data = await SendAsync(mutation, new
        {
            input = new
            {
                cartItemId = itemId,
                quantity   = req.Quantity
            }
        });

        var itemEl = data.GetProperty("updateCartItem");
        if (itemEl.ValueKind == JsonValueKind.Null)
            return null;

        var cartId = itemEl.GetProperty("cartId").GetInt32();
        return await GetByIdAsync(cartId);
    }

    public async Task<bool> RemoveItemAsync(int itemId)
    {
        const string mutation = """
            mutation($id: Int!) {
              deleteCartItem(id: $id)
            }
            """;

        var data = await SendAsync(mutation, new { id = itemId });
        return data.GetProperty("deleteCartItem").GetBoolean();
    }

    public async Task<bool> DeleteCartAsync(int cartId)
    {
        const string mutation = """
            mutation($id: Int!) {
              deleteCart(id: $id)
            }
            """;

        var data = await SendAsync(mutation, new { id = cartId });
        return data.GetProperty("deleteCart").GetBoolean();
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
