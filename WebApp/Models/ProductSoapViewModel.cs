using System.ComponentModel.DataAnnotations;
using FakeStore.WebApp.SoapClients;

namespace FakeStore.WebApp.Models;

public class ProductSoapViewModel
{
    public string searchedTerm { get; set; } = String.Empty;

    [Range(0, double.MinValue, ErrorMessage = "Min price must be 0 or more.")]
    public double? selectedMinPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Max price must be 0 or more.")]

    public double? selectedMaxPrice { get; set; }
    public bool found { get; set; }
    public string? errorMessage { get; set; }
    public SearchResult searchResult { get; set; } = new SearchResult();
}