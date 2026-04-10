using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FakeStore.ViewModel;
using MyRestApi.DTO;
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
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<ProductRead>> Create([FromBody] ProductCreate req)
    {
        var created = await _service.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = created.product_id }, created);
    }

    // PUT /api/product/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "full access")]
    public async Task<ActionResult<ProductRead>> Update(int id, [FromBody] ProductUpdate req)
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
    public async Task<ActionResult<ReviewProductRead>> AddReview(int id, [FromBody] ReviewProductCreate req)
    {
        var userId = 1;

        var reviewDto = reviewViewToReviewDTO(id, userId, req);


        var review = await _service.AddReviewAsync(reviewDto);
        return review is null ? NotFound() : CreatedAtAction(nameof(GetById), new { id }, review);
    }

    private ReviewApiDto reviewViewToReviewDTO(int productId, int userId, ReviewProductCreate view)
    {
        return new ReviewApiDto
        {
            ProductId = productId,
            UserId = userId,
            Comment = view.Comment,
            Rating = view.Rating
        };
    }
}
