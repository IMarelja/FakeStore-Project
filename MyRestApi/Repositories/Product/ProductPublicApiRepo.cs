using System.Text.Json;
using FakeStore.View;
using FakeStore.ViewModel;
using MyRestApi.DTO;
using MyRestApi.Middleware;

namespace MyRestApi.Repositories;

public class ProductPublicApiRepo(IHttpClientFactory factory) : IProductRepo
{
    private readonly HttpClient _http = factory.CreateClient("publicapi");

    private static readonly JsonSerializerOptions _readOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<Product>> GetAllAsync()
    {
        try
        {
            var dtos = await _http.GetFromJsonAsync<List<ProductApiDto>>("products", _readOptions) ?? [];
            return dtos.Select(ToModel).ToList();
        }
        catch (HttpRequestException ex)
        {
            throw new RestApiServiceException("REST API service is unavailable.", ex);
        }
    }

    private static Product ToModel(ProductApiDto dto) => new()
    {
        ProductId   = dto.ProductId,
        Name        = dto.Name,
        Description = dto.Description,
        Price       = dto.Price,
        Unit        = dto.Unit,
        Image       = dto.Image,
        Discount    = dto.Discount,
        Available   = dto.Availability,
        Brand       = dto.Brand,
        Rating      = dto.Rating,
        Reviews     = dto.Reviews.Select(r => new Review
        {
            UserId  = r.UserId,
            Rating  = r.Rating,
            Comment = r.Comment
        }).ToList()
    };

    public async Task<Product?> GetByIdAsync(int id)
    {
        throw new MyRestApi.Middleware.NotImplementedException("Not implemented");
    }

    public async Task<Product> CreateAsync(ProductCreate req)
    {
        throw new MyRestApi.Middleware.NotImplementedException("Not implemented");
    }

    public async Task<Product?> UpdateAsync(int id, ProductUpdate req)
    {
        throw new MyRestApi.Middleware.NotImplementedException("Not implemented");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        throw new MyRestApi.Middleware.NotImplementedException("Not implemented");
    }
}
