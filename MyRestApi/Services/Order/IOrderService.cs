using MyRestApi.DTO.Order;

namespace MyRestApi.Services;

public interface IOrderService
{
    Task<List<OrderReadDto>> GetAllAsync();
    Task<OrderReadDto?> GetByIdAsync(int orderId);
    Task<OrderReadDto> CreateAsync(OrderCreateDto req);
    Task<OrderReadDto?> UpdateAsync(int orderId, OrderUpdateDto req);
    Task<bool> DeleteAsync(int orderId);
}
