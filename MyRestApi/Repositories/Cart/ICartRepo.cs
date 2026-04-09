using FakeStore.Models;
using FakeStore.ViewModel;

namespace MyRestApi.Repositories;

public interface ICartRepo
{
    Task<List<Cart>> GetAllAsync();
    Task<Cart?> GetByIdAsync(int cartId);
    Task<Cart?> GetByUserIdAsync(int userId);
    Task<Cart?> AddItemAsync(int cartId, CartItemAdd req);
    Task<Cart?> EditItemAsync(int itemId, CartItemEdit req);
    Task<bool> RemoveItemAsync(int itemId);
    Task<bool> RemoveItemByUserAndProductAsync(int userId, int productId);
    Task<bool> DeleteCartAsync(int cartId);
    Task<Cart?> AddUserCartAsync(int userId);
}
