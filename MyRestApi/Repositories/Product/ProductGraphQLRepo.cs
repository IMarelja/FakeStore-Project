using System.Text;
using System.Text.Json;
using FakeStore.Models;
using MyRestApi.DTO;
using MyRestApi.DTO.Product;
using MyRestApi.Middleware;

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

    public async Task<List<Product>> GetAllAsync()
    {
        const string query = """
            {
              products {
                productId
                name
                description
                price
                unit
                image
                discount
                available
                brand
                rating
                category
                reviews { reviewId userId rating comment }
              }
            }
            """;

        var data = await SendAsync(query);
        return JsonSerializer.Deserialize<List<Product>>(
            data.GetProperty("products").GetRawText(), _readOptions)!;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        const string query = """
            query($id: Int!) {
              product(id: $id) {
                productId
                name
                description
                price
                unit
                image
                discount
                available
                brand
                rating
                category
                reviews {
                    reviewId
                    userId
                    rating
                    comment
                }
              }
            }
            """;

        var data = await SendAsync(query, new { id });
        var productEl = data.GetProperty("product");

        if (productEl.ValueKind == JsonValueKind.Null)
            return null;

        return JsonSerializer.Deserialize<Product>(productEl.GetRawText(), _readOptions)!;
    }

    public async Task<Product> CreateAsync(ProductCreateDto req)
    {
        const string mutation = """
            mutation createProduct($input: ProductInput!) {
              createProduct(input: $input) {
                productId
                name
                description
                price
                unit
                image
                discount
                available
                brand
                rating
                category
              }
            }
            """;

        var data = await SendAsync(mutation, new
        {
            input = new
            {
                req.Name,
                req.Description,
                req.Price,
                req.Unit,
                req.Image,
                req.Discount,
                req.Available,
                req.Brand,
                req.Category,
                Rating = 0.0
            }
        });
        return JsonSerializer.Deserialize<Product>(
            data.GetProperty("createProduct").GetRawText(), _readOptions)!;
    }

    public async Task<Product?> UpdateAsync(int id, ProductUpdateDto req)
    {
        const string mutation = """
            mutation($id: Int!, $input: ProductInput!) {
              updateProduct(id: $id, input: $input) {
                productId
                name
                description
                price
                unit
                image
                discount
                available
                brand
                rating
                category
              }
            }
            """;

        var data = await SendAsync(mutation, new
        {
            input = new
            {
                id,
                req.Name,
                req.Description,
                req.Price,
                req.Unit,
                req.Image,
                req.Discount,
                req.Available,
                req.Brand,
                req.Category,
                Rating = 0.0
            }
        });
        return JsonSerializer.Deserialize<Product>(
            data.GetProperty("updateProduct").GetRawText(), _readOptions)!;
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

    public async Task<Review?> AddReviewAsync(ReviewApiDto dto)
    {
        const string mutation = """
            mutation($input: ReviewInput!) {
              createReview(input: $input) {
                reviewId
                productId
                userId
                rating
                comment
              }
            }
            """;

        var data = await SendAsync(mutation, new
        {
            input = new
            {
                UserId    = dto.UserId,
                ProductId = dto.ProductId,
                Rating    = dto.Rating,
                Comment   = dto.Comment
            }
        });

        var el = data.GetProperty("createReview");
        return el.ValueKind == JsonValueKind.Null
            ? null
            : JsonSerializer.Deserialize<Review>(el.GetRawText(), _readOptions);
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
