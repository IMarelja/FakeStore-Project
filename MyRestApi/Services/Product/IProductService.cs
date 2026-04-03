using FakeStore.ViewModel;

namespace MyRestApi.Services;

public interface IProductService
{
    Task<List<ProductRead>> GetAllAsync();
    Task<ProductRead?> GetByIdAsync(int id);
    Task<ProductRead> CreateAsync(ProductCreate req);
    Task<ProductRead?> UpdateAsync(int id, ProductUpdate req);
    Task<bool> DeleteAsync(int id);
}