using FakeStore.View;
using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public interface IProductRepo
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(ProductCreate req);
    Task<Product?> UpdateAsync(int id, ProductUpdate req);
    Task<bool> DeleteAsync(int id);
}
