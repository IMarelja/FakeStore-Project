using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.DTO;

namespace MyRestApi.Repositories;

public interface IProductRepo
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(ProductCreate req);
    Task<Product?> UpdateAsync(int id, ProductUpdate req);
    Task<bool> DeleteAsync(int id);
    Task<Review?> AddReviewAsync(ReviewApiDto dto);
}
