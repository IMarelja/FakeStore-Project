using FakeStore.ViewModel;

namespace WebApp.Service;

public class ProductCustomApiService : IProductService
{
    public Task<ReviewProductRead> AddReview(ReviewProductCreate review)
    {
        throw new NotImplementedException();
    }

    public Task<ProductRead> CreateProduct(ProductCreate product)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ProductRead>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<ProductRead?> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ProductRead?> UpdateProduct(int id, ProductUpdate product)
    {
        throw new NotImplementedException();
    }
}