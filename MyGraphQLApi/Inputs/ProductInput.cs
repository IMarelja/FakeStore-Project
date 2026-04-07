namespace MyGraphQLApi.GraphQL;

public record ProductInput(
    string Name,
    string Description,
    decimal Price,
    string Unit,
    string Image,
    int Discount,
    bool Available,
    string Brand,
    string Category,
    double Rating
);