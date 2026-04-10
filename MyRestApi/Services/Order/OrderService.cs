using FakeStore.Models;
using MyRestApi.DTO.Order;
using MyRestApi.Repositories;

namespace MyRestApi.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepo _repo;

    public OrderService(IOrderRepo repo)
    {
        _repo = repo;
    }

    public async Task<List<OrderReadDto>> GetAllAsync()
    {
        var orders = await _repo.GetAllAsync();
        return orders.Select(ToDto).ToList();
    }

    public async Task<OrderReadDto?> GetByIdAsync(int orderId)
    {
        var order = await _repo.GetByIdAsync(orderId);
        return order is null ? null : ToDto(order);
    }

    public async Task<OrderReadDto> CreateAsync(OrderCreateDto req)
    {
        var order = await _repo.CreateAsync(req);
        return ToDto(order);
    }

    public async Task<OrderReadDto?> UpdateAsync(int orderId, OrderUpdateDto req)
    {
        var order = await _repo.UpdateAsync(orderId, req);
        return order is null ? null : ToDto(order);
    }

    public Task<bool> DeleteAsync(int orderId) =>
        _repo.DeleteAsync(orderId);

    private static OrderReadDto ToDto(Order o) => new()
    {
        order_id    = o.OrderId,
        user_id     = o.UserId,
        status      = o.OrderStatus,
        total_price = (double)o.TotalPrice,
        items       = o.Items.Select(i => new OrderItemReadDto
        {
            product_id = i.ProductId,
            quantity   = i.Quantity
        }).ToList()
    };
}
