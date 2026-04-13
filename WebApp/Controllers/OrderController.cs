using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace FakeStore.WebApp.Controllers;


public class OrderController : Controller
{
    private const string HomeFeedbackMessageKey = "HomeFeedbackMessage";
    private const string HomeFeedbackIsErrorKey = "HomeFeedbackIsError";

    private readonly IOrderService _orderService;
    private readonly ApiRuntimeMode _apiRuntimeMode;

    public OrderController(IOrderService orderService, ApiRuntimeMode apiRuntimeMode)
    {
        _orderService = orderService;
        _apiRuntimeMode = apiRuntimeMode;
    }

    [HttpGet]
    public async Task<IActionResult> TabData()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var orders = await _orderService.GetAll();

            var sortedOrders = currentUserId.HasValue
                ? orders.OrderBy(order => order.user_id == currentUserId.Value ? 0 : 1)
                    .ThenByDescending(order => order.order_id)
                : orders.OrderByDescending(order => order.order_id);

            var vm = new OrderTabCardsViewModel
            {
                Orders = sortedOrders,
                CurrentUserId = currentUserId,
                HasFullAccessRole = HasFullAccessRole(),
                IsPublicApi = _apiRuntimeMode.IsPublicMode
            };

            return PartialView("_OrderCards", vm);
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                BuildErrorMessage("Could not load orders.", ex));
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
            SetHomeFeedback("Order status cannot be empty.", isError: true);
            return RedirectToHomeTab();
        }

        try
        {
            var updatedOrder = await _orderService.EditOrder(orderId, status.Trim());
            SetHomeFeedback($"Order #{updatedOrder.OrderId} status updated to {updatedOrder.Status}.", isError: false);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            SetHomeFeedback("Order not found.", isError: true);
        }
        catch (Exception ex)
        {
            SetHomeFeedback(BuildErrorMessage("Could not update order status.", ex), isError: true);
        }

        return RedirectToHomeTab();
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
            SetHomeFeedback(
                deleted ? $"Order #{orderId} deleted." : "Order not found.",
                isError: !deleted);
        }
        catch (Exception ex)
        {
            SetHomeFeedback(BuildErrorMessage("Could not delete order.", ex), isError: true);
        }

        return RedirectToHomeTab();
    }

    private IActionResult RedirectToHomeTab()
    {
        return RedirectToAction("Index", "Home", new { tab = "orders" });
    }

    private void SetHomeFeedback(string message, bool isError)
    {
        TempData[HomeFeedbackMessageKey] = message;
        TempData[HomeFeedbackIsErrorKey] = isError;
    }

    private bool HasFullAccessRole()
    {
        return User?.Claims.Any(c =>
            c.Type == ClaimTypes.Role
            && c.Value.Equals("full access", StringComparison.OrdinalIgnoreCase)) ?? false;
    }

    private int? GetCurrentUserId()
    {
        var userIdValue = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }

    private static string BuildErrorMessage(string prefix, Exception ex)
    {
        if (ex is HttpRequestException requestException && !string.IsNullOrWhiteSpace(requestException.Message))
        {
            return $"{prefix} {requestException.Message}";
        }

        return prefix;
    }
}
