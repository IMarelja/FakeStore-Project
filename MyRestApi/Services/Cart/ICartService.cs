using FakeStore.ViewModel;

namespace MyRestApi.Services;

public interface ICartService
{
    Task<List<CartRead>> GetAllAsync();
    Task<CartRead?> GetByIdAsync(int cartId);
    Task<CartRead?> GetByUserIdAsync(int userId);
    Task<CartRead?> AddItemAsync(int cartId, CartItemAdd req);
    Task<CartRead?> EditItemAsync(int itemId, CartItemEdit req);
    Task<bool> RemoveItemAsync(int itemId);
    Task<bool> RemoveItemByUserAndProductAsync(int userId, int productId);
    Task<bool> DeleteCartAsync(int cartId);
}
