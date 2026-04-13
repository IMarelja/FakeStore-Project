namespace MyGraphQLApi.GraphQL;

public record ProductInput(
    string Name,
    string Description,
    decimal Price,
    int Discount,
    bool Available,
    string Brand,
    string Category,
    double Rating = 0,
    string? Unit = null,
    string? Image = null
);
