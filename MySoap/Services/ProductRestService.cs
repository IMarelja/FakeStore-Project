using System.Net.Http.Json;
using FakeStore.ViewModel;

namespace MySoap.Services;

public class ProductRestService : IProductRestService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;

    public ProductRestService(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpFactory = httpFactory;
        _config = config;
    }

    public async Task<List<ProductRead>> GetAll()
    {
        using var http = _httpFactory.CreateClient();

        var baseUrl = _config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException(
                "Missing API base URL. Configure Api:BaseUrl or ConnectionStrings:DefaultConnection.");

        var response = await http.GetAsync($"{baseUrl}/products");

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Get products failed ({(int)response.StatusCode} {response.ReasonPhrase}). {body}");
        }

        var products = await response.Content.ReadFromJsonAsync<List<ProductRead>>();
        return products ?? [];
    }

    public Task ToXmlFile(List<ProductRead> products)
    {
        throw new NotImplementedException();
    }

    public Task<bool> VerifyXmlFile()
    {
        throw new NotImplementedException();
    }
}
