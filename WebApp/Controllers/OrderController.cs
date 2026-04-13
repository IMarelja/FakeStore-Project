using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FakeStore.WebApp.Controllers;


public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ApiRuntimeMode _apiRuntimeMode;
    private readonly IHomeService _homeService;
    private readonly IJwtService _jwtService;

    public OrderController(
        IOrderService orderService,
        ApiRuntimeMode apiRuntimeMode,
        IHomeService homeService,
        IJwtService jwtService)
    {
        _orderService = orderService;
        _apiRuntimeMode = apiRuntimeMode;
        _homeService = homeService;
        _jwtService = jwtService;
    }

    [HttpGet]
    public async Task<IActionResult> TabData()
    {
        try
        {
            var currentUserId = _jwtService.GetCurrectUserId();
            var orders = await _orderService.GetAll();

            var sortedOrders = currentUserId.HasValue
                ? orders.OrderBy(order => order.user_id == currentUserId.Value ? 0 : 1)
                    .ThenByDescending(order => order.order_id)
                : orders.OrderByDescending(order => order.order_id);

            var vm = new OrderTabCardsViewModel
            {
                Orders = sortedOrders,
                CurrentUserId = currentUserId,
                HasFullAccessRole = _jwtService.HasFullAccessRole(),
                IsPublicApi = _apiRuntimeMode.IsPublicMode
            };

            return PartialView("_OrderCards", vm);
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                _homeService.BuildHomeErrorMessage("Could not load orders.", ex));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "FullAccessRoleOnly")]
    [Authorize(Policy = "CustomApiOnly")]
    public async Task<IActionResult> UpdateStatus(int orderId, string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            _homeService.SetFeedback(TempData, "Order status cannot be empty.", isError: true);
            return _homeService.RedirectToHomeTab("orders");
        }

        try
        {
            var updatedOrder = await _orderService.EditOrder(orderId, status.Trim());
            _homeService.SetFeedback(TempData, $"Order #{updatedOrder.OrderId} status updated to {updatedOrder.Status}.", isError: false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _homeService.SetFeedback(TempData, "Order not found.", isError: true);
        }
        catch (Exception ex)
        {
            _homeService.SetFeedback(TempData, _homeService.BuildHomeErrorMessage("Could not update order status.", ex), isError: true);
        }

        return _homeService.RedirectToHomeTab("orders");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "FullAccessRoleOnly")]
    [Authorize(Policy = "CustomApiOnly")]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        try
        {
            var deleted = await _orderService.DeleteOrder(orderId);
            _homeService.SetFeedback(TempData,
                deleted ? $"Order #{orderId} deleted." : "Order not found.",
                isError: !deleted);
        }
        catch (Exception ex)
        {
            _homeService.SetFeedback(TempData, _homeService.BuildHomeErrorMessage("Could not delete order.", ex), isError: true);
        }

        return _homeService.RedirectToHomeTab("orders");
    }
}
