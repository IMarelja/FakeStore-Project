using FakeStore.ViewModel;

namespace FakeStore.WebApp.Service;

public interface ICartService
{
    Task<IEnumerable<CartRead>> getAll();
    Task<CartRead?> getById(int id);
    Task<CartRead?> getOwnCart();
    Task<CartRead> addItemToOwnCart(CartItemAdd item);
    Task<CartRead?> editItemToOwnCart(int cartItemId, int quantity);
    Task<bool> deleteItemFromOwnCart(int productId);
    Task<bool> deleteOwnCart();
    Task<bool> deleteCart(int cartId);
}
