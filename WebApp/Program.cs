using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Service;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authentication.Cookies;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// -----------------------------------------
//  Weather gRPC clinet
// -----------------------------------------

var grpcAddress = configuration["GrpcServer"]!;
builder.Services.AddGrpcClient<global::WebApp.gRPC.WeatherService.WeatherServiceClient>(o =>
{
    o.Address = new Uri(grpcAddress);
});

// -----------------------------------------
//  Selection between API's
// -----------------------------------------

var apiSelector = configuration.GetSection(ApiSelectorOptions.SectionName).Get<ApiSelectorOptions>()
    ?? throw new InvalidOperationException("Missing ApiSelector configuration.");

var selectedApi = apiSelector.Selected.Trim();
var isPublicMode = selectedApi.Equals("Public", StringComparison.OrdinalIgnoreCase);
var isCustomMode = selectedApi.Equals("Custom", StringComparison.OrdinalIgnoreCase);

if (!isPublicMode && !isCustomMode)
{
    throw new InvalidOperationException("ApiSelector:Selected must be either 'Public' or 'Custom'.");
}

var runtimeSelectedApi = isCustomMode ? "Custom" : "Public";
builder.Services.AddSingleton(new ApiRuntimeMode(runtimeSelectedApi, isPublicMode, isCustomMode));

builder.Services.AddHttpClient("ApiClient", client =>
{
    var baseUrl = isCustomMode ? apiSelector.Apis.Custom : apiSelector.Apis.Public;

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException($"Missing base URL for '{selectedApi}' API mode.");
    }

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddScoped<IAuthenticationService, AuthenticationCustomApiService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
        options.SlidingExpiration = true;
    });

if (isPublicMode)
{
    builder.Services.AddScoped<IProductService, ProductPublicApiService>();
    builder.Services.AddScoped<IUserService, UserPublicApiService>();
    builder.Services.AddScoped<IOrderService, OrderPublicApiService>();
    builder.Services.AddScoped<ICartService, CartPublicApiService>();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("CustomApiOnly", policy => policy.RequireAssertion(_ => isCustomMode));
        options.AddPolicy("PublicApiOnly", policy => policy.RequireAssertion(_ => isPublicMode));
        options.AddPolicy("FullAccessRoleOnly", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("ReadOnlyRole", policy => policy.RequireAssertion(_ => true));
    });
}
if (isCustomMode)
{
    builder.Services.AddScoped<IProductService, ProductCustomApiService>();
    builder.Services.AddScoped<IUserService, UserCustomApiService>();
    builder.Services.AddScoped<IOrderService, OrderCustomApiService>();
    builder.Services.AddScoped<ICartService, CartCustomApiService>();
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("CustomApiOnly", policy => policy.RequireAssertion(_ => isCustomMode));
        options.AddPolicy("PublicApiOnly", policy => policy.RequireAssertion(_ => isPublicMode));
        options.AddPolicy("FullAccessRoleOnly", policy => policy.RequireRole("full access"));
        options.AddPolicy("ReadOnlyRole", policy => policy.RequireRole("read-only", "full access"));
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
