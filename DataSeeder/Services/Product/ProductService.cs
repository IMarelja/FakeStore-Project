using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Data;
using DataSeeder.Repositories;
using FakeStore.Models;
using FakeStore.ViewModel;
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

        var apiProducts = await http.GetFromJsonAsync<List<ProductRead>>($"{baseUrl}products", _jsonOptions) ?? [];

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
