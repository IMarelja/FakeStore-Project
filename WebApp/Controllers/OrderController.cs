using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

[Authorize]
[Authorize(Policy = "CustomApiOnly")]
[Authorize(Policy = "ReadOnlyRole")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> TabData()
    {
        try
        {
            var orders = await _orderService.GetOwn();
            return PartialView("_OrderCards", orders.OrderByDescending(order => order.order_id));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not load your orders.");
        }
    }
}
