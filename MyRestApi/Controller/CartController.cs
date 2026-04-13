using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.DTO.Cart;
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
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<IEnumerable<CartReadDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET /api/cart/{id}
    [HttpGet("{id:int}")]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<CartReadDto>> GetById(int id)
    {
        var cart = await _service.GetByIdAsync(id);
        return cart is null ? NotFound() : Ok(cart);
    }

    // GET /api/cart/me
    [HttpGet("me")]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<CartReadDto>> GetOwnCart()
    {
        var cart = await _service.GetByUserIdAsync(_claims.GetUserId());
        return cart is null ? NotFound() : Ok(cart);
    }

    // POST /api/cart/me/item
    [HttpPost("me/item")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<CartReadDto>> AddItemToOwnCart([FromBody] CartItemAddDto req)
    {
        var own = await _service.GetByUserIdAsync(_claims.GetUserId());
        if (own is null) return NotFound();

        var cart = await _service.AddItemAsync(own.cart_id, req);
        return cart is null ? NotFound() : CreatedAtAction(nameof(GetOwnCart), cart);
    }

    // PUT /api/cart/me/{id}/item
    [HttpPut("me/{id:int}/item")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<CartReadDto>> EditItemToOwnCart(int id, [FromBody] CartItemEditDto req)
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

    // DELETE /api/cart/me
    [HttpDelete("me")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteOwnCart()
    {
        var own = await _service.GetByUserIdAsync(_claims.GetUserId());
        if (own is null) return NotFound();

        return await _service.DeleteCartAsync(own.cart_id) ? NoContent() : NotFound();
    }

    // DELETE /api/cart/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteCart(int id)
    {
        return await _service.DeleteCartAsync(id) ? NoContent() : NotFound();
    }
}
