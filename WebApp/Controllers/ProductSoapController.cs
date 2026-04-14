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
    public IActionResult Index()
    {
        return View(new ProductSoapViewModel());
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

        try
        {
            var result = await _service.Quary(vm.searchedTerm.Trim(), vm.selectedMinPrice, vm.selectedMaxPrice);
            vm.searchResult = result ?? EmptySearchResult();
            vm.searchResult.Products ??= [];
            vm.found = true;
        }
        catch (Exception ex)
        {
            vm.found = true;
            vm.errorMessage = $"Search failed. {ex.Message}";
            vm.searchResult = EmptySearchResult();
        }

        return View(vm);
    }

    private static SearchResult EmptySearchResult() => new()
    {
        Products = []
    };
}
