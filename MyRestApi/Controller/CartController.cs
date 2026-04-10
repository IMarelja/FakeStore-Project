using FakeStore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _service;
    private readonly IClaimsService _claims;

    public CartController(ICartService service, IClaimsService claims)
    {
        _service = service;
        _claims  = claims;
    }

    // GET /api/cart
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartRead>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET /api/cart/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CartRead>> GetById(int id)
    {
        var cart = await _service.GetByIdAsync(id);
        return cart is null ? NotFound() : Ok(cart);
    }

    // GET /api/cart/me
    [HttpGet("me")]
    public async Task<ActionResult<CartRead>> GetOwnCart()
    {
        var cart = await _service.GetByUserIdAsync(_claims.GetUserId());
        return cart is null ? NotFound() : Ok(cart);
    }

    // POST /api/cart/me/item
    [HttpPost("me/item")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<CartRead>> AddItemToOwnCart([FromBody] CartItemAdd req)
    {
        var own = await _service.GetByUserIdAsync(_claims.GetUserId());
        if (own is null) return NotFound();

        var cart = await _service.AddItemAsync(own.cart_id, req);
        return cart is null ? NotFound() : CreatedAtAction(nameof(GetOwnCart), cart);
    }

    // PUT /api/cart/me/{id}/item
    [HttpPut("me/{id:int}/item")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<CartRead>> EditItemToOwnCart(int id, [FromBody] CartItemEdit req)
    {
        var cart = await _service.EditItemAsync(id, req);
        return cart is null ? NotFound() : Ok(cart);
    }

    // DELETE /api/cart/me/item
    [HttpDelete("me/item")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteItemToOwnCart([FromQuery] int product_id)
    {
        return await _service.RemoveItemByUserAndProductAsync(_claims.GetUserId(), product_id)
            ? NoContent() : NotFound();
    }

    // POST /api/cart/{id}/item
    [HttpPost("{id:int}/item")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<CartRead>> AddItemToCart(int id, [FromBody] CartItemAdd req)
    {
        var cart = await _service.AddItemAsync(id, req);
        return cart is null ? NotFound() : CreatedAtAction(nameof(GetById), new { id }, cart);
    }

    // PUT /api/cart/{id}/item
    [HttpPut("{id:int}/item")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<CartRead>> EditItemToCart(int id, [FromBody] CartItemEdit req)
    {
        var cart = await _service.EditItemAsync(id, req);
        return cart is null ? NotFound() : Ok(cart);
    }

    // DELETE /api/cart/{id}/item
    [HttpDelete("{id:int}/item")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteItemToCart(int id, [FromQuery] int product_id)
    {
        return await _service.RemoveItemByUserAndProductAsync(_claims.GetUserId(), product_id)
            ? NoContent() : NotFound();
    }

    // DELETE /api/cart
    [HttpDelete]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteCart()
    {
        var own = await _service.GetByUserIdAsync(_claims.GetUserId());
        if (own is null) return NotFound();

        return await _service.DeleteCartAsync(own.cart_id) ? NoContent() : NotFound();
    }
}
