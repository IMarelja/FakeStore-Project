using FakeStore.ViewModel;
using FakeStore.WebApp.Configuration;
using FakeStore.WebApp.Models;
using FakeStore.WebApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.WebApp.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly ApiRuntimeMode _apiRuntimeMode;
    private readonly IHomeService _homeService;
    private readonly IJwtService _jwtService;

    public CartController(
        ICartService cartService,
        IOrderService orderService,
        ApiRuntimeMode apiRuntimeMode,
        IHomeService homeService,
        IJwtService jwtService)
    {
        _cartService = cartService;
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
            var carts = await _cartService.getAll();
            var orderedCarts = currentUserId.HasValue
                ? carts.OrderBy(cart => cart.user_id == currentUserId.Value ? 0 : 1).ThenBy(cart => cart.cart_id)
                : carts.OrderBy(cart => cart.cart_id);

            var vm = new CartTabCardsViewModel
            {
                Carts = orderedCarts,
                CurrentUserId = currentUserId,
                HasFullAccessRole = _jwtService.HasFullAccessRole(),
                IsPublicApi = _apiRuntimeMode.IsPublicMode
            };

            return PartialView("_CartCards", vm);
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                _homeService.BuildHomeErrorMessage("Could not load carts.", ex));
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
            _homeService.SetFeedback(TempData, "Quantity must be greater than zero.", isError: true);
            return _homeService.RedirectToHomeTab("products");
        }

        try
        {
            await _cartService.addItemToOwnCart(new CartItemAdd
            {
                ProductId = productId,
                Quantity = quantity
            });
            _homeService.SetFeedback(TempData, "Item added to your cart.", isError: false);
        }
        catch (Exception ex)
        {
            _homeService.SetFeedback(TempData, _homeService.BuildHomeErrorMessage("Could not add item to cart.", ex), isError: true);
        }

        return _homeService.RedirectToHomeTab("products");
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
            _homeService.SetFeedback(TempData, "Quantity must be greater than zero.", isError: true);
            return _homeService.RedirectToHomeTab("cart");
        }

        try
        {
            var updated = await _cartService.editItemToOwnCart(cartItemId, quantity);
            if (updated is null)
            {
                _homeService.SetFeedback(TempData, "Cart item not found.", isError: true);
                return _homeService.RedirectToHomeTab("cart");
            }

            _homeService.SetFeedback(TempData, "Item quantity updated.", isError: false);
        }
        catch (Exception ex)
        {
            _homeService.SetFeedback(TempData, _homeService.BuildHomeErrorMessage("Could not update item quantity.", ex), isError: true);
        }

        return _homeService.RedirectToHomeTab("cart");
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
            _homeService.SetFeedback(TempData,
                deleted ? "Item removed from your cart." : "Cart item not found.",
                isError: !deleted);
        }
        catch (Exception ex)
        {
            _homeService.SetFeedback(TempData, _homeService.BuildHomeErrorMessage("Could not remove item from cart.", ex), isError: true);
        }

        return _homeService.RedirectToHomeTab("cart");
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
            _homeService.SetFeedback(TempData,
                deleted ? "Your cart has been wiped." : "Your cart was not found.",
                isError: !deleted);
        }
        catch (Exception ex)
        {
            _homeService.SetFeedback(TempData, _homeService.BuildHomeErrorMessage("Could not wipe your cart.", ex), isError: true);
        }

        return _homeService.RedirectToHomeTab("cart");
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
                _homeService.SetFeedback(TempData, "Your cart was not found.", isError: true);
                return _homeService.RedirectToHomeTab("cart");
            }

            if (ownCart.items.Count == 0)
            {
                _homeService.SetFeedback(TempData, "Your cart is empty.", isError: true);
                return _homeService.RedirectToHomeTab("cart");
            }

            if (ownCart.items.Any(item => item.quantity <= 0))
            {
                _homeService.SetFeedback(TempData, "Cart contains invalid item quantities.", isError: true);
                return _homeService.RedirectToHomeTab("cart");
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
            _homeService.SetFeedback(TempData, $"Order #{created.OrderId}, Status: {created.Status}, Message: {created.Message}", isError: false);
        }
        catch (Exception ex)
        {
            _homeService.SetFeedback(TempData, _homeService.BuildHomeErrorMessage("Could not place order.", ex), isError: true);
        }

        return _homeService.RedirectToHomeTab("cart");
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
            _homeService.SetFeedback(TempData,
                deleted ? $"Cart #{cartId} deleted." : "Cart not found.",
                isError: !deleted);
        }
        catch (Exception ex)
        {
            _homeService.SetFeedback(TempData, _homeService.BuildHomeErrorMessage("Could not delete cart.", ex), isError: true);
        }

        return _homeService.RedirectToHomeTab("cart");
    }
}
