using System.ServiceModel;
using MySoap.Models;

namespace MySoap.Services;

[ServiceContract]
public interface IProductSoapService
{
    [OperationContract]
    Task<SearchResult> SearchProductsAsync(string term);

    [OperationContract]
    Task<ValidationResult> ValidateProductsAsync();
}
