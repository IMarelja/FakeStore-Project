using Microsoft.EntityFrameworkCore;
using MyGraphQLApi.Data;
using MyGraphQLApi.GraphQL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FakeStoreDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

app.MapGraphQL();

app.Run();
