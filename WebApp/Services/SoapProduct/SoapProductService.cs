using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public class SoapProductService : ISoapProductService
{
    public async Task<List<ProductSoapRead>> Quary(ProductSoapQuary quary)
    {
        throw new NotImplementedException();
    }
}

