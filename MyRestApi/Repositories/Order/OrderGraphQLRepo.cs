using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.Middleware;

namespace MyRestApi.Repositories;

public class OrderGraphQLRepo : IOrderRepo
{
    public Task<List<Order>> GetAllAsync() => 
        throw new Exception("GetAll orders is not yet implemented.");

    public Task<Order?> GetByIdAsync(int orderId) =>
        throw new Exception("GetById order is not yet implemented.");

    public Task<Order> CreateAsync(OrderCreate req) =>
        throw new Exception("CreateOrder is not yet implemented.");

    public Task<Order?> UpdateAsync(int orderId, OrderUpdate req) =>
        throw new Exception("UpdateOrder is not yet implemented.");

    public Task<bool> DeleteAsync(int orderId) =>
        throw new Exception("DeleteOrder is not yet implemented.");
}
