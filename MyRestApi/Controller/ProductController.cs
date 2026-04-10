using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRestApi.DTO;
using MyRestApi.DTO.Product;
using MyRestApi.Services;

namespace MyRestApi.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    // GET /api/product
    [HttpGet]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    // GET /api/product/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "read-only,full access")]
    public async Task<ActionResult<ProductReadDto>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    // POST /api/product
    [HttpPost]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<ProductReadDto>> Create([FromBody] ProductCreateDto req)
    {
        var created = await _service.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = created.product_id }, created);
    }

    // PUT /api/product/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<ProductReadDto>> Update(int id, [FromBody] ProductUpdateDto req)
    {
        var updated = await _service.UpdateAsync(id, req);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE /api/product/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }

    // POST /api/product/{id}/review
    [HttpPost("{id}/review")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<ReviewProductReadDto>> AddReview(int id, [FromBody] ReviewProductCreateDto req)
    {
        var userId = 1;

        var reviewDto = ReviewToApiDto(id, userId, req);

        var review = await _service.AddReviewAsync(reviewDto);
        return review is null ? NotFound() : CreatedAtAction(nameof(GetById), new { id }, review);
    }

    private static ReviewApiDto ReviewToApiDto(int productId, int userId, ReviewProductCreateDto req)
    {
        return new ReviewApiDto
        {
            ProductId = productId,
            UserId    = userId,
            Comment   = req.Comment,
            Rating    = req.Rating
        };
    }
}
