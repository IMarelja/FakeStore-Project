using Microsoft.AspNetCore.Mvc;
using FakeStore.ViewModel;
using MyRestApi.Repositories;

namespace MyRestApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepo _repo;

    public ProductController(IProductRepo repo)
    {
        _repo = repo;
    }

    // GET /api/product
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductRead>>> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    // GET /api/product/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductRead>> GetById(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    // POST /api/product
    [HttpPost]
    public async Task<ActionResult<ProductRead>> Create([FromBody] ProductCreate req)
    {
        var created = await _repo.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = created.product_id }, created);
    }

    // PUT /api/product/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductRead>> Update(int id, [FromBody] ProductUpdate req)
    {
        var updated = await _repo.UpdateAsync(id, req);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE /api/product/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _repo.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
