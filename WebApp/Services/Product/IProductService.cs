using System.Collections;
using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public interface IProductService
{
    Task<IEnumerable<ProductRead>> GetAll();
    Task<ProductRead?> GetById(int id);
    Task<ProductRead> CreateProduct(ProductCreate product);
    Task<ProductRead?> UpdateProduct(int id, ProductUpdate product);
    Task<ReviewProductRead> AddReview(ReviewProductCreate review);

}
