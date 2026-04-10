using MyRestApi.DTO.Cart;

namespace MyRestApi.Services;

public interface ICartService
{
    Task<List<CartReadDto>> GetAllAsync();
    Task<CartReadDto?> GetByIdAsync(int cartId);
    Task<CartReadDto?> GetByUserIdAsync(int userId);
    Task<CartReadDto?> AddItemAsync(int cartId, CartItemAddDto req);
    Task<CartReadDto?> EditItemAsync(int itemId, CartItemEditDto req);
    Task<bool> RemoveItemAsync(int itemId);
    Task<bool> RemoveItemByUserAndProductAsync(int userId, int productId);
    Task<bool> DeleteCartAsync(int cartId);
}
