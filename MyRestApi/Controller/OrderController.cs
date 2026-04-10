using FakeStore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    // GET /api/orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderRead>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET /api/orders/status?order_id={order_id}
    [HttpGet("status")]
    public async Task<ActionResult<OrderStatusRead>> GetById([FromQuery] int order_id)
    {
        var order = await _service.GetByIdAsync(order_id);
        return order is null ? NotFound() : Ok(ToStatusModel(order));
    }

    // PUT /api/orders/ (⚠️ PUT MUST CREATE ORDERS, I KNOW IT IS BAD, BUT IT MUST)
    [HttpPut]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<OrderCreateResponse>> AddOrder([FromBody] OrderCreate req)
    {
        var created = await _service.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { created.order_id }, ToCreateResponceModel(created, "Order successfully placed."));
    }

    // POST /api/orders/{id} (⚠️ POST MUST UPDATE ORDERS, I KNOW IT IS BAD, BUT IT MUST)
    [HttpPost("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<OrderCreateResponse>> EditOrder(int id, [FromBody] OrderUpdate req)
    {
        var updated = await _service.UpdateAsync(id, req);
        return updated is null ? NotFound() : Ok(ToCreateResponceModel(updated, "Order successfully updated."));
    }

    // DELETE /api/orders/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        return await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    private static OrderStatusRead ToStatusModel(OrderRead o) => new()
    {
        order_id    = o.order_id,
        user_id     = o.user_id,
        status      = o.status,
        total_price = o.total_price
    };

    private static OrderCreateResponse ToCreateResponceModel(OrderRead o, string message) => new()
    {
        OrderId     = o.order_id,
        Status      = o.status,
        Message     = message
    };
}
