using MyRestApi.DTO;
using MyRestApi.DTO.Product;

namespace MyRestApi.Services;

public interface IProductService
{
    Task<List<ProductReadDto>> GetAllAsync();
    Task<ProductReadDto?> GetByIdAsync(int id);
    Task<ProductReadDto> CreateAsync(ProductCreateDto req);
    Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto req);
    Task<bool> DeleteAsync(int id);
    Task<ReviewProductReadDto?> AddReviewAsync(ReviewApiDto dto);
}
