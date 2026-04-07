using FakeStore.ViewModel;

namespace MyRestApi.Services;

public interface IOrderService
{
    Task<List<OrderRead>> GetAllAsync();
    Task<OrderRead?> GetByIdAsync(int orderId);
    Task<OrderRead> CreateAsync(OrderCreate req);
    Task<OrderRead?> UpdateAsync(int orderId, OrderUpdate req);
    Task<bool> DeleteAsync(int orderId);
}
