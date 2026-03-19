using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public interface IProductRepo
{
    Task<List<ProductRead>> GetAllAsync();
    Task<ProductRead?> GetByIdAsync(int id);
    Task<ProductRead> CreateAsync(ProductCreate req);
    Task<ProductRead?> UpdateAsync(int id, ProductUpdate req);
    Task<bool> DeleteAsync(int id);
}
