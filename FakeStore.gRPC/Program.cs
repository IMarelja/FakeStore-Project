using FakeStore.gRPC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGrpcService<WeatherService>();
app.MapGet("/", () => "gRPC server running.");

app.Run();
