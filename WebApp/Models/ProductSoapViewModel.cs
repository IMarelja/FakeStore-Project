using FakeStore.WebApp.SoapClients;

namespace FakeStore.WebApp.Models;

public class ProductSoapViewModel
{
    public string searchedTerm { get; set; } = String.Empty;
    public double? selectedMinPrice { get; set; }
    public double? selectedMaxPrice { get; set; }
    public bool Found { get; set; }
    public SearchResult searchResult { get; set; } = new SearchResult();
}