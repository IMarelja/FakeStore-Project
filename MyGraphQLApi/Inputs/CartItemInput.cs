namespace MyGraphQLApi.GraphQL;

public record CartItemInput(
    int CartId, 
    int ProductId, 
    int Quantity
);