namespace MyGraphQLApi.GraphQL;

public record OrderItemInput(int OrderId, int ProductId, int Quantity);