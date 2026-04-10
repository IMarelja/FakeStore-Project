using FakeStore.Models;
using MyRestApi.DTO.Order;

namespace MyRestApi.Repositories;

public interface IOrderRepo
{
    Task<List<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int orderId);
    Task<Order> CreateAsync(OrderCreateDto req);
    Task<Order?> UpdateAsync(int orderId, OrderUpdateDto req);
    Task<bool> DeleteAsync(int orderId);
}
