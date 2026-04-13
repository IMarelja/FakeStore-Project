using MySoap.Services;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSoapCore();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IProductRestService, ProductRestService>();
builder.Services.AddSingleton<IProductSoapService, ProductSoapService>();

var app = builder.Build();

app.UseSoapEndpoint<IProductSoapService>("/ProductService.asmx", new SoapEncoderOptions());

app.Run();
