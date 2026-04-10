using FakeStore.Models;
using MyRestApi.DTO;
using MyRestApi.DTO.Product;

namespace MyRestApi.Repositories;

public interface IProductRepo
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(ProductCreateDto req);
    Task<Product?> UpdateAsync(int id, ProductUpdateDto req);
    Task<bool> DeleteAsync(int id);
    Task<Review?> AddReviewAsync(ReviewApiDto dto);
}
