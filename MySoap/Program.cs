using MySoap.Services;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSoapCore();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IProductRestService, ProductRestService>();
builder.Services.AddSingleton<IProductSoapService, ProductSoapService>();

var app = builder.Build();

try
{
    var restService = app.Services.GetRequiredService<IProductRestService>();
    var products = await restService.GetAll();
    await restService.ToXmlFile(products);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    throw;
}

app.UseSoapEndpoint<IProductSoapService>("/ProductService.asmx", new SoapEncoderOptions());

app.Run();
