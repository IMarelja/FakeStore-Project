using System.ServiceModel;
using MySoap.Models;

namespace MySoap.Services;

[ServiceContract]
public interface IProductSoapService
{
    Task<SearchResult> SearchProductsAsync(string? term, double? minPrice, double? maxPrice);
}
