using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DataSeeder.Models;
using DataSeeder.Repositories;
using FakeStore.View;
using Microsoft.Extensions.Configuration;

namespace DataSeeder.Services;

public class ProductService(IHttpClientFactory httpFactory, IProductRepository repo, IConfiguration config) : IProductService
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndSeedAsync()
    {
        using var http = httpFactory.CreateClient();
        var baseUrl = config["Api:BaseUrl"]!;

        var apiProducts = await http.GetFromJsonAsync<List<ProductApiModel>>($"{baseUrl}products", _jsonOptions) ?? [];

        var products = apiProducts.Select(p => new Product
        {
            ProductId   = p.ProductId,
            Name        = p.Name,
            Description = p.Description,
            Price       = p.Price,
            Unit        = p.Unit,
            Image       = p.Image,
            Discount    = p.Discount,
            Available   = p.Availability,
            Brand       = p.Brand,
            Rating      = p.Rating
        });

        var reviews = apiProducts.SelectMany(p => p.Reviews.Select(r => new Review
        {
            ProductId = p.ProductId,
            UserId    = r.UserId,
            Rating    = r.Rating,
            Comment   = r.Comment
        }));

        await repo.SeedAsync(products, reviews);
    }
}
