using FakeStore.ViewModel;

namespace WebApp.Service;

public interface IOrderService
{
    Task<IEnumerable<OrderRead>> GetAll();
    Task<OrderRead?> GetById(int id);
    Task<OrderCreateResponse> AddOrder(OrderCreate orderCreate);
    Task<OrderCreateResponse> EditOrder(string status);
}
