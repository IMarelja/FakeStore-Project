using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class ProductSoapController : Controller
{
    private readonly ISoapProductService _service;
    

    public ProductSoapController(ISoapProductService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        var vm = new ProductSoapViewModel();

        return View();
    }
}