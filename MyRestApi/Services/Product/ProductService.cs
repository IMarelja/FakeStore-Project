using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.DTO;
using MyRestApi.Repositories;

namespace MyRestApi.Services;

public class ProductService : IProductService
{
    private readonly IProductRepo _repo;

    public ProductService(IProductRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductRead>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(ToViewModel).ToList();
    }

    public async Task<ProductRead?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? null : ToViewModel(product);
    }

    public async Task<ProductRead> CreateAsync(ProductCreate req)
    {
        var product = await _repo.CreateAsync(req);
        return ToViewModel(product);
    }

    public async Task<ProductRead?> UpdateAsync(int id, ProductUpdate req)
    {
        var product = await _repo.UpdateAsync(id, req);
        return product is null ? null : ToViewModel(product);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    public async Task<ReviewProductRead?> AddReviewAsync(ReviewApiDto dto)
    {
        var review = await _repo.AddReviewAsync(dto);
        if (review is null) return null;
        return new ReviewProductRead
        {
            user_id = review.UserId,
            rating  = review.Rating,
            comment = review.Comment
        };
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
        category     = p.Category,
        reviews      = p.Reviews.Select(r => new ReviewProductRead
        {
            user_id = r.UserId,
            rating  = r.Rating,
            comment = r.Comment
        }).ToList()
    };
}