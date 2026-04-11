using FakeStore.Models;

namespace MyGraphQLApi.GraphQL;

public record OrderInput(
    int UserId, 
    string OrderStatus, 
    decimal TotalPrice,
    List<OrderItemOrderInput> Items
);

public record OrderItemOrderInput(
    int ProductId, 
    int Quantity
);
