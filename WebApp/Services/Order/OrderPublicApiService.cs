using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public class OrderPublicApiService : IOrderService
{
    public Task<OrderCreateResponse> AddOrder(OrderCreate orderCreate)
    {
        throw new NotImplementedException();
    }

    public Task<OrderCreateResponse> EditOrder(string status)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderRead>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<OrderRead?> GetById(int id)
    {
        throw new NotImplementedException();
    }
}