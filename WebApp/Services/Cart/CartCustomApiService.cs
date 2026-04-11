using FakeStore.ViewModel;

namespace WebApp.Service;

public class CartCustomApiService : ICartService
{
    public Task<CartRead> addItemToOwnCart(CartItemAdd item)
    {
        throw new NotImplementedException();
    }

    public Task<bool> deleteItemFromOwnCart(int productId)
    {
        throw new NotImplementedException();
    }

    public Task<CartRead?> editItemToOwnCart(int productId, int quantity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CartRead>> getAll()
    {
        throw new NotImplementedException();
    }

    public Task<CartRead?> getById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<CartRead?> getOwnCart()
    {
        throw new NotImplementedException();
    }
}