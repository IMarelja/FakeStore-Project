using FakeStore.Models;
using MyRestApi.DTO.Cart;

namespace MyRestApi.Repositories;

public interface ICartRepo
{
    Task<List<Cart>> GetAllAsync();
    Task<Cart?> GetByIdAsync(int cartId);
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart?> AddItemAsync(int cartId, CartItemAddDto req);
    Task<Cart?> EditItemAsync(int cartId, int productId, CartItemEditDto req);
    Task<bool> RemoveItemAsync(int itemId);
    Task<bool> RemoveItemByUserAndProductAsync(int userId, int productId);
    Task<bool> DeleteCartAsync(int cartId);
    Task<Cart?> AddUserCartAsync(int userId);
}
