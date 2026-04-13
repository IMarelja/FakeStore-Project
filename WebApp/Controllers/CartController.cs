using System.Security.Claims;
using FakeStore.ViewModel;
using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class CartController : Controller
{
    private const string HomeFeedbackMessageKey = "HomeFeedbackMessage";
    private const string HomeFeedbackIsErrorKey = "HomeFeedbackIsError";

    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly ApiRuntimeMode _apiRuntimeMode;

    public CartController(
        ICartService cartService,
        IOrderService orderService,
        ApiRuntimeMode apiRuntimeMode)
    {
        _cartService = cartService;
        _orderService = orderService;
        _apiRuntimeMode = apiRuntimeMode;
    }

    [HttpGet]
    public async Task<IActionResult> TabData()
    {
        if (!CanAccessTabScreen())
        {
            return StatusCode(StatusCodes.Status403Forbidden, "You need to sign-up to view this page");
        }

        try
        {
            var currentUserId = GetCurrentUserId();
            var carts = await _cartService.getAll();
            var orderedCarts = currentUserId.HasValue
                ? carts.OrderBy(cart => cart.user_id == currentUserId.Value ? 0 : 1).ThenBy(cart => cart.cart_id)
                : carts.OrderBy(cart => cart.cart_id);

            var vm = new CartTabCardsViewModel
            {
                Carts = orderedCarts,
                CurrentUserId = currentUserId,
                HasFullAccessRole = HasFullAccessRole()
            };

            return PartialView("_CartCards", vm);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not load carts.");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> AddItemToOwnCart(int productId, int quantity)
    {
        if (quantity <= 0)
        {
            SetHomeFeedback("Quantity must be greater than zero.", isError: true);
            return RedirectToHomeTab("products");
        }

        try
        {
            await _cartService.addItemToOwnCart(new CartItemAdd
            {
                ProductId = productId,
                Quantity = quantity
            });
            SetHomeFeedback("Item added to your cart.", isError: false);
        }
        catch (Exception ex)
        {
            SetHomeFeedback(BuildErrorMessage("Could not add item to cart.", ex), isError: true);
        }

        return RedirectToHomeTab("products");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> UpdateOwnItemQuantity(int cartItemId, int quantity)
    {
        if (quantity <= 0)
        {
            SetHomeFeedback("Quantity must be greater than zero.", isError: true);
            return RedirectToHomeTab("cart");
        }

        try
        {
            var updated = await _cartService.editItemToOwnCart(cartItemId, quantity);
            if (updated is null)
            {
                SetHomeFeedback("Cart item not found.", isError: true);
                return RedirectToHomeTab("cart");
            }

            SetHomeFeedback("Item quantity updated.", isError: false);
        }
        catch (Exception ex)
        {
            SetHomeFeedback(BuildErrorMessage("Could not update item quantity.", ex), isError: true);
        }

        return RedirectToHomeTab("cart");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> DeleteOwnItem(int productId)
    {
        try
        {
            var deleted = await _cartService.deleteItemFromOwnCart(productId);
            SetHomeFeedback(
                deleted ? "Item removed from your cart." : "Cart item not found.",
                isError: !deleted);
        }
        catch (Exception ex)
        {
            SetHomeFeedback(BuildErrorMessage("Could not remove item from cart.", ex), isError: true);
        }

        return RedirectToHomeTab("cart");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> WipeOwnCart()
    {
        try
        {
            var deleted = await _cartService.deleteOwnCart();
            SetHomeFeedback(
                deleted ? "Your cart has been wiped." : "Your cart was not found.",
                isError: !deleted);
        }
        catch (Exception ex)
        {
            SetHomeFeedback(BuildErrorMessage("Could not wipe your cart.", ex), isError: true);
        }

        return RedirectToHomeTab("cart");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> PlaceOwnOrder()
    {
        try
        {
            var ownCart = await _cartService.getOwnCart();
            if (ownCart is null)
            {
                SetHomeFeedback("Your cart was not found.", isError: true);
                return RedirectToHomeTab("cart");
            }

            if (ownCart.items.Count == 0)
            {
                SetHomeFeedback("Your cart is empty.", isError: true);
                return RedirectToHomeTab("cart");
            }

            if (ownCart.items.Any(item => item.quantity <= 0))
            {
                SetHomeFeedback("Cart contains invalid item quantities.", isError: true);
                return RedirectToHomeTab("cart");
            }

            var created = await _orderService.AddOrder(new OrderCreate
            {
                UserId = ownCart.user_id,
                Items = ownCart.items.Select(item => new ItemCartRead
                {
                    product_id = item.product_id,
                    quantity = item.quantity
                }).ToList()
            });

            await _cartService.deleteOwnCart();
            SetHomeFeedback($"Order #{created.OrderId}, Status: {created.Status}, Message: {created.Message}", isError: false);
        }
        catch (Exception ex)
        {
            SetHomeFeedback(BuildErrorMessage("Could not place order.", ex), isError: true);
        }

        return RedirectToHomeTab("cart");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [Authorize(Policy = "CustomApiOnly")]
    [Authorize(Policy = "FullAccessRoleOnly")]
    public async Task<IActionResult> DeleteCart(int cartId)
    {
        try
        {
            var deleted = await _cartService.deleteCart(cartId);
            SetHomeFeedback(
                deleted ? $"Cart #{cartId} deleted." : "Cart not found.",
                isError: !deleted);
        }
        catch (Exception ex)
        {
            SetHomeFeedback(BuildErrorMessage("Could not delete cart.", ex), isError: true);
        }

        return RedirectToHomeTab("cart");
    }

    private IActionResult RedirectToHomeTab(string tabKey)
    {
        return RedirectToAction("Index", "Home", new { tab = tabKey });
    }

    private void SetHomeFeedback(string message, bool isError)
    {
        TempData[HomeFeedbackMessageKey] = message;
        TempData[HomeFeedbackIsErrorKey] = isError;
    }

    private bool CanAccessTabScreen()
    {
        var isSignedIn = User?.Identity?.IsAuthenticated ?? false;
        return _apiRuntimeMode.IsPublicMode || (_apiRuntimeMode.IsCustomMode && isSignedIn);
    }

    private bool HasFullAccessRole()
    {
        return User?.Claims.Any(claim =>
            claim.Type == ClaimTypes.Role
            && claim.Value.Equals("full access", StringComparison.OrdinalIgnoreCase)) ?? false;
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
