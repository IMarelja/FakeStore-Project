using FakeStore.Models;
using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public interface IOrderRepo
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int orderId);
    Task<Order> CreateAsync(OrderCreate req);
    Task<Order?> UpdateAsync(int orderId, OrderUpdate req);
    Task<bool> DeleteAsync(int orderId);
}
