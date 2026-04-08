using MySoap.Services;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSoapCore();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IProductSoapService, ProductSoapService>();

var app = builder.Build();

app.UseSoapEndpoint<IProductSoapService>("/ProductService.asmx", new SoapEncoderOptions());

app.Run();
