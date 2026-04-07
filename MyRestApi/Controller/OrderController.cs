using FakeStore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[Route("api/[controller]")]
[ApiController]
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

    // POST /api/orders/
    [HttpPost]
    public async Task<ActionResult<OrderCreateResponse>> AddOrder([FromBody] OrderCreate req)
    {
        var created = await _service.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { created.order_id }, created);
    }

    // PUT /api/orders/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderCreateResponse>> EditOrder(int id, [FromBody] OrderUpdate req)
    {
        var updated = await _service.UpdateAsync(id, req);

        if(updated is null)
            NotFound();

        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE /api/orders/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        return await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    private static OrderStatusRead ToStatusModel(OrderRead o) => new()
    {
        order_id    = o.order_id,
        user_id     = o.user_id,
        status      = o.status,
        total_price = o.total_price,
        items       = o.items
    };

    private static OrderCreateResponse ToCreateResponceModel(OrderRead o) => new()
    {
        OrderId     = o.order_id,
        Status      = o.status,
        Message     = "Order successfuly something"
    };
}
