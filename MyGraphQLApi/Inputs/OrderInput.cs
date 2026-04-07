namespace MyGraphQLApi.GraphQL;

public record OrderInput(
    int UserId, 
    string OrderStatus, 
    decimal TotalPrice
);