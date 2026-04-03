using Microsoft.AspNetCore.Mvc;
using FakeStore.ViewModel;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    // GET /api/product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductRead>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET /api/product/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductRead>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    // POST /api/product
    [HttpPost]
    public async Task<ActionResult<ProductRead>> Create([FromBody] ProductCreate req)
    {
        var created = await _service.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = created.product_id }, created);
    }

    // PUT /api/product/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductRead>> Update(int id, [FromBody] ProductUpdate req)
    {
        var updated = await _service.UpdateAsync(id, req);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE /api/product/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
