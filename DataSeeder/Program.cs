using DataSeeder.Data;
using DataSeeder.Repositories;
using DataSeeder.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


// --------------------------------------------
//      Is extra seeded sql file here?
// --------------------------------------------
var sqlPath = Path.Combine(AppContext.BaseDirectory, "seed-extra.sql");
if (!File.Exists(sqlPath))
{
    Console.WriteLine("seed-extra.sql not found, skipping extra seeding.");
    return;
}

// --------------------------------------
//      Host connection setup
// --------------------------------------
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
var cfg = sp.GetRequiredService<IConfiguration>();


// --------------------------------------------
//      API and Database connection check
// --------------------------------------------
Console.WriteLine("Checking database connection...");
if (!await ctx.Database.CanConnectAsync())
{
    Console.Error.WriteLine("ERROR: Cannot connect to the database. Check your connection string.");
    return;
}
Console.WriteLine("Database connection OK.");

Console.WriteLine("Checking API connection...");
var baseUrl = cfg["Api:BaseUrl"];
using (var http = new HttpClient())
{
    try
    {
        var response = await http.GetAsync(baseUrl);
        if (!response.IsSuccessStatusCode)
        {
            Console.Error.WriteLine($"ERROR: API returned {(int)response.StatusCode} {response.ReasonPhrase}. Check the API URL.");
            return;
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"ERROR: Cannot reach the API at {baseUrl}. {ex.Message}");
        return;
    }
}
Console.WriteLine("API connection OK.");



// -----------------------------------------
//      Actually inputing data
// -----------------------------------------
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

Console.WriteLine("Resetting sequences...");
await ctx.Database.ExecuteSqlRawAsync(@"
    SELECT setval('users_user_id_seq',       (SELECT MAX(user_id)    FROM users));
    SELECT setval('product_product_id_seq',  (SELECT MAX(product_id) FROM product));
    SELECT setval('orders_order_id_seq',     (SELECT MAX(order_id)   FROM orders));
    SELECT setval('cart_cart_id_seq',        (SELECT MAX(cart_id)    FROM cart));
");

Console.WriteLine("Running extra SQL seed file...");
var sql = await File.ReadAllTextAsync(sqlPath);
await ctx.Database.ExecuteSqlRawAsync(sql);

Console.WriteLine("Done! Database seeded successfully.");
