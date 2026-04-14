using FakeStore.ViewModel;
using FakeStore.WebApp.SoapClients;

namespace FakeStore.WebApp.Service;

public interface ISoapProductService
{
    Task<SearchResult> Quary(string term, double? minPrice, double? maxPrice);
    
}