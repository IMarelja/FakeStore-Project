using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public interface IOrderService
{
    Task<IEnumerable<OrderRead>> GetAll();
    Task<IEnumerable<OrderRead>> GetOwn();
    Task<OrderRead?> GetById(int id);
    Task<OrderCreateResponse> AddOrder(OrderCreate orderCreate);
    Task<OrderCreateResponse> EditOrder(int id, string status);
}
