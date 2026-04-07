
namespace MyGraphQLApi.GraphQL;

public record ReviewInput(int UserId, int ProductId, int Rating, string Comment);