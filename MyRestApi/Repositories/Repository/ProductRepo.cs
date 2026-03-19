using System.Text;
using System.Text.Json;
using FakeStore.View;
using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public class ProductRepo : IProductRepo
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

    public ProductRepo(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("graphql");
    }

    private static ProductRead ToViewModel(Product p) => new()
    {
        product_id   = p.ProductId,
        name         = p.Name,
        description  = p.Description,
        price        = (double)p.Price,
        unit         = p.Unit,
        image        = p.Image,
        discount     = p.Discount,
        availability = p.Available,
        brand        = p.Brand,
        rating       = p.Rating,
        reviews      = p.Reviews.Select(r => new ReviewProductRead
        {
            user_id = r.UserId,
            rating  = r.Rating,
            comment = r.Comment
        }).ToList()
    };

    private async Task<JsonElement> SendAsync(string query, object? variables = null)
    {
        var body = JsonSerializer.Serialize(new { query, variables }, _writeOptions);
        var response = await _http.PostAsync("graphql", new StringContent(body, Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement.GetProperty("data").Clone();
    }

    public async Task<List<ProductRead>> GetAllAsync()
    {
        const string query = """
            {
              products {
                productId name description price unit image discount available brand rating
                reviews { reviewId userId rating comment }
              }
            }
            """;

        var data = await SendAsync(query);
        var products = JsonSerializer.Deserialize<List<Product>>(
            data.GetProperty("products").GetRawText(), _readOptions)!;

        return products.Select(ToViewModel).ToList();
    }

    public async Task<ProductRead?> GetByIdAsync(int id)
    {
        const string query = """
            query($id: Int!) {
              product(id: $id) {
                productId name description price unit image discount available brand rating
                reviews { reviewId userId rating comment }
              }
            }
            """;

        var data = await SendAsync(query, new { id });
        var productEl = data.GetProperty("product");

        if (productEl.ValueKind == JsonValueKind.Null)
            return null;

        return ToViewModel(JsonSerializer.Deserialize<Product>(productEl.GetRawText(), _readOptions)!);
    }

    public async Task<ProductRead> CreateAsync(ProductCreate req)
    {
        const string query = """
            mutation($input: ProductInput!) {
              createProduct(input: $input) {
                productId name description price unit image discount available brand rating
              }
            }
            """;

        var data = await SendAsync(query, new { input = req });
        var product = JsonSerializer.Deserialize<Product>(
            data.GetProperty("createProduct").GetRawText(), _readOptions)!;

        return ToViewModel(product);
    }

    public async Task<ProductRead?> UpdateAsync(int id, ProductUpdate req)
    {
        const string query = """
            mutation($id: Int!, $input: ProductInput!) {
              updateProduct(id: $id, input: $input) {
                productId name description price unit image discount available brand rating
              }
            }
            """;

        var data = await SendAsync(query, new { id, input = req });
        var updatedEl = data.GetProperty("updateProduct");

        if (updatedEl.ValueKind == JsonValueKind.Null)
            return null;

        return ToViewModel(JsonSerializer.Deserialize<Product>(updatedEl.GetRawText(), _readOptions)!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string query = """
            mutation($id: Int!) {
              deleteProduct(id: $id)
            }
            """;

        var data = await SendAsync(query, new { id });
        return data.GetProperty("deleteProduct").GetBoolean();
    }
}
