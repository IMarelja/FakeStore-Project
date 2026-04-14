using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using FakeStore.WebApp.SoapClients;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class ProductSoapController : Controller
{
    private readonly ISoapProductService _service;

    public ProductSoapController(ISoapProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new ProductSoapViewModel();
        await LoadResults(vm, string.Empty, null, null);
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Index(ProductSoapViewModel vm)
    {
        if (vm.selectedMinPrice.HasValue
            && vm.selectedMaxPrice.HasValue
            && vm.selectedMinPrice.Value > vm.selectedMaxPrice.Value)
        {
            ModelState.AddModelError(nameof(vm.selectedMaxPrice), "Max price must be greater than or equal to min price.");
        }

        if (!ModelState.IsValid)
        {
            vm.searchResult = EmptySearchResult();
            return View(vm);
        }

        await LoadResults(vm, (vm.searchedTerm ?? string.Empty).Trim(), vm.selectedMinPrice, vm.selectedMaxPrice);
        return View(vm);
    }

    private async Task LoadResults(ProductSoapViewModel vm, string term, double? minPrice, double? maxPrice)
    {
        try
        {
            var result = await _service.Quary(term, minPrice, maxPrice);
            vm.searchResult = result ?? EmptySearchResult();
            vm.searchResult.Products ??= [];
            vm.found = true;
            vm.errorMessage = null;
        }
        catch (Exception ex)
        {
            vm.found = true;
            vm.errorMessage = $"Search failed. {ex.Message}";
            vm.searchResult = EmptySearchResult();
        }
    }

    private static SearchResult EmptySearchResult() => new()
    {
        Products = []
    };
}
