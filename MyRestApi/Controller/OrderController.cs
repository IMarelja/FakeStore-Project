using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.DTO.Order;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    private readonly IClaimsService _claims;

    public OrdersController(IOrderService service, IClaimsService claims)
    {
        _service = service;
        _claims = claims;
    }

    // GET /api/orders
    [HttpGet]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET /api/orders/me
    [HttpGet("me")]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOwn()
    {
        var userId = _claims.GetUserId();
        var allOrders = await _service.GetAllAsync();
        return Ok(allOrders.Where(order => order.user_id == userId));
    }

    // GET /api/orders/status?order_id={order_id}
    [HttpGet("status")]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<OrderStatusReadDto>> GetById([FromQuery] int order_id)
    {
        var order = await _service.GetByIdAsync(order_id);
        return order is null ? NotFound() : Ok(ToStatusDto(order));
    }

    // PUT /api/orders/ (⚠️ PUT MUST CREATE ORDERS, I KNOW IT IS BAD, BUT IT MUST)
    [HttpPut]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<OrderCreateResponseDto>> AddOrder([FromBody] OrderCreateDto req)
    {
        var created = await _service.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { created.order_id }, ToCreateResponseDto(created, "Order successfully placed."));
    }

    // POST /api/orders/{id} (⚠️ POST MUST UPDATE ORDERS, I KNOW IT IS BAD, BUT IT MUST)
    [HttpPost("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<OrderCreateResponseDto>> EditOrder(int id, [FromBody] OrderUpdateDto req)
    {
        var updated = await _service.UpdateAsync(id, req);
        return updated is null ? NotFound() : Ok(ToCreateResponseDto(updated, "Order successfully updated."));
    }

    // DELETE /api/orders/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        return await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    private static OrderStatusReadDto ToStatusDto(OrderReadDto o) => new()
    {
        order_id    = o.order_id,
        user_id     = o.user_id,
        status      = o.status,
        total_price = o.total_price
    };

    private static OrderCreateResponseDto ToCreateResponseDto(OrderReadDto o, string message) => new()
    {
        OrderId = o.order_id,
        Status  = o.status,
        Message = message
    };
}
