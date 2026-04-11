using System.Text;
using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

var grpcAddress = configuration["GrpcServer"]!;
builder.Services.AddGrpcClient<FakeStore.gRPC.WeatherService.WeatherServiceClient>(o =>
{
    o.Address = new Uri(grpcAddress);
});

var apiSelector = configuration.GetSection(ApiSelectorOptions.SectionName).Get<ApiSelectorOptions>()
    ?? throw new InvalidOperationException("Missing ApiSelector configuration.");

var selectedApi = apiSelector.Selected.Trim();
var isPublicMode = selectedApi.Equals("Public", StringComparison.OrdinalIgnoreCase);
var isCustomMode = selectedApi.Equals("Custom", StringComparison.OrdinalIgnoreCase);

if (!isPublicMode && !isCustomMode)
{
    throw new InvalidOperationException("ApiSelector:Selected must be either 'Public' or 'Custom'.");
}

builder.Services.AddHttpClient("ApiClient", client =>
{
    var baseUrl = isCustomMode ? apiSelector.Apis.Custom : apiSelector.Apis.Public;

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException($"Missing base URL for '{selectedApi}' API mode.");
    }

    client.BaseAddress = new Uri(baseUrl);
});

if (isPublicMode)
{
    builder.Services.AddScoped<IProductService, ProductPublicApiService>();
    builder.Services.AddScoped<IUserService, UserPublicApiService>();
    builder.Services.AddScoped<IOrderService, OrderPublicApiService>();
    builder.Services.AddScoped<ICartService, CartPublicApiService>();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("WeatherReadOnly", policy => policy.RequireAssertion(_ => true));
    });
}
else
{
    builder.Services.AddScoped<IProductService, ProductCustomApiService>();
    builder.Services.AddScoped<IUserService, UserCustomApiService>();
    builder.Services.AddScoped<IOrderService, OrderCustomApiService>();
    builder.Services.AddScoped<ICartService, CartCustomApiService>();
    builder.Services.AddScoped<IAuthenticationService, AuthenticationCustomApiService>();

    var jwtSection = configuration.GetSection("Jwt");
    var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key configuration.");
    var jwtIssuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Missing Jwt:Issuer configuration.");
    var jwtAudience = jwtSection["Audience"] ?? throw new InvalidOperationException("Missing Jwt:Audience configuration.");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("WeatherReadOnly", policy => policy.RequireRole("read-only"));
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

if (isCustomMode)
{
    app.UseAuthentication();
}

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
