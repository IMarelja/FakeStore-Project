using DataSeeder.Data;
using DataSeeder.Repositories;
using DataSeeder.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(cfg =>
        cfg.AddJsonFile("appsettings.json", optional: false))
    .ConfigureServices((ctx, services) =>
    {
        services.AddDbContext<PostgresDbContext>(options =>
            options.UseNpgsql(ctx.Configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        services.AddHttpClient();

        services.AddScoped<IUserRepository,    UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository,    CartRepository>();
        services.AddScoped<IOrderRepository,   OrderRepository>();

        services.AddScoped<IUserService,    UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService,    CartService>();
        services.AddScoped<IOrderService,   OrderService>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var sp  = scope.ServiceProvider;
var ctx = sp.GetRequiredService<PostgresDbContext>();

Console.WriteLine("Clearing existing data...");
await ctx.Database.ExecuteSqlRawAsync(
    "TRUNCATE TABLE users, product RESTART IDENTITY CASCADE");

Console.WriteLine("Seeding users...");
await sp.GetRequiredService<IUserService>().FetchAndSeedAsync();

Console.WriteLine("Seeding products and reviews...");
await sp.GetRequiredService<IProductService>().FetchAndSeedAsync();

Console.WriteLine("Seeding carts...");
await sp.GetRequiredService<ICartService>().FetchAndSeedAsync();

Console.WriteLine("Seeding orders...");
await sp.GetRequiredService<IOrderService>().FetchAndSeedAsync();

Console.WriteLine("Done! Database seeded successfully.");
