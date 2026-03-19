namespace MyGraphQLApi.GraphQL;

public record UserInput(string Username, string Email, string Password);

public record ProductInput(
    string Name,
    string Description,
    decimal Price,
    string Unit,
    string Image,
    int Discount,
    bool Available,
    string Brand,
    double Rating);

public record ReviewInput(int UserId, int ProductId, int Rating, string Comment);

public record CartInput(int UserId);

public record CartItemInput(int CartId, int ProductId, int Quantity);

public record OrderInput(int UserId, int OrderStatus, decimal TotalPrice);
public record OrderItemInput(int OrderId, int ProductId, int Quantity);
