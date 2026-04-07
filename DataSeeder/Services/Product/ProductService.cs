using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using DataSeeder.Data;
using DataSeeder.Repositories;
using FakeStore.Models;
using FakeStore.ViewModel;
using Json.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class ProductService(IHttpClientFactory httpFactory, IProductRepository repo, IConfiguration config, PostgresDbContext ctx) : IProductService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var raw = await http.GetStringAsync($"{baseUrl}products");

        var schemaPath = Path.Combine(AppContext.BaseDirectory, "JsonSchemas", "Product.json");
        var schema = JsonSchema.FromText(await File.ReadAllTextAsync(schemaPath));
        var result = schema.Evaluate(JsonNode.Parse(raw), new EvaluationOptions { OutputFormat = OutputFormat.List });

        if (!result.IsValid)
        {
            var errors = result.Details
                .Where(d => !d.IsValid && d.Errors is not null)
                .SelectMany(d => d.Errors!.Select(e => $"  {d.InstanceLocation}: {e.Value}"));

            throw new InvalidDataException($"Product API response failed schema validation:\n{string.Join("\n", errors)}");
        }

        var apiProducts = JsonSerializer.Deserialize<List<ProductRead>>(raw, _jsonOptions) ?? [];

        var products = apiProducts.Select(p => new Product
        {
            ProductId   = p.product_id,
            Name        = p.name,
            Description = p.description,
            Price       = (decimal)p.price,
            Unit        = p.unit,
            Image       = p.image,
            Discount    = p.discount,
            Available   = p.availability,
            Brand       = p.brand,
            Category    = p.category,
            Rating      = p.rating
        });

        var validUserIds = (await ctx.Users.Select(u => u.UserId).ToListAsync()).ToHashSet();

        var reviews = apiProducts.SelectMany(p => p.reviews
            .Where(r => validUserIds.Contains(r.user_id))
            .Select(r => new Review
            {
                ProductId = p.product_id,
                UserId    = r.user_id,
                Rating    = r.rating,
                Comment   = r.comment
            }));

        await repo.SeedAsync(products, reviews);
    }
}
