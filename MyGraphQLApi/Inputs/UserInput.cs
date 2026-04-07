namespace MyGraphQLApi.GraphQL;

public record UserInput(
    string Username, 
    string Email, 
    string Password, 
    string Role = "read-only"
);