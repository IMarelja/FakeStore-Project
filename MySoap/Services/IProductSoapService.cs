using System.ServiceModel;
using MySoap.Models;

namespace MySoap.Services;

[ServiceContract]
public interface IProductSoapService
{
    [OperationContract]
    Task<SearchResult> SearchProducts(string? term, double? minPrice, double? maxPrice);
}
