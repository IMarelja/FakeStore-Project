using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public interface ISoapProductService
{
    Task<List<ProductSoapRead>> Quary(ProductSoapQuary quary);
}