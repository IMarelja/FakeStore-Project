namespace MyGraphQLApi.GraphQL;

public record OrderUpdateInput(
    int? UserId,
    string? OrderStatus,
    decimal? TotalPrice
);
