using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.Repositories;

namespace MyRestApi.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepo _repo;

    public OrderService(IOrderRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<OrderRead>> GetAllAsync()
    {
        var orders = await _repo.GetAllAsync();
        return orders.Select(ToViewModel).ToList();
    }

    public async Task<OrderRead?> GetByIdAsync(int orderId)
    {
        var order = await _repo.GetByIdAsync(orderId);
        return order is null ? null : ToViewModel(order);
    }

    public async Task<OrderRead> CreateAsync(OrderCreate req)
    {
        var order = await _repo.CreateAsync(req);
        return ToViewModel(order);
    }

    public async Task<OrderRead?> UpdateAsync(int orderId, OrderUpdate req)
    {
        var order = await _repo.UpdateAsync(orderId, req);
        return order is null ? null : ToViewModel(order);
    }

    public Task<bool> DeleteAsync(int orderId) =>
        _repo.DeleteAsync(orderId);

    private static OrderRead ToViewModel(Order o) => new()
    {
        order_id    = o.OrderId,
        user_id     = o.UserId,
        status      = o.OrderStatus,
        total_price = (double)o.TotalPrice,
        items       = o.Items.Select(i => new ItemOrderRead
        {
            product_id = i.ProductId,
            quantity   = i.Quantity
        }).ToList()
    };
}
