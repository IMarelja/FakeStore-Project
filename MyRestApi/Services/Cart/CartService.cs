using FakeStore.Models;
using FakeStore.ViewModel;
using MyRestApi.Repositories;

namespace MyRestApi.Services;

public class CartService(ICartRepo repo) : ICartService
{
    public async Task<List<CartRead>> GetAllAsync()
    {
        var carts = await repo.GetAllAsync();
        return carts.Select(ToViewModel).ToList();
    }

    public async Task<CartRead?> GetByIdAsync(int cartId)
    {
        var cart = await repo.GetByIdAsync(cartId);
        return cart is null ? null : ToViewModel(cart);
    }

    public async Task<CartRead?> GetByUserIdAsync(int userId)
    {
        var cart = await repo.GetByUserIdAsync(userId);
        return cart is null ? null : ToViewModel(cart);
    }

    public async Task<CartRead?> AddItemAsync(int cartId, CartItemAdd req)
    {
        var cart = await repo.AddItemAsync(cartId, req);
        return cart is null ? null : ToViewModel(cart);
    }

    public async Task<CartRead?> EditItemAsync(int itemId, CartItemEdit req)
    {
        var cart = await repo.EditItemAsync(itemId, req);
        return cart is null ? null : ToViewModel(cart);
    }

    public Task<bool> RemoveItemAsync(int itemId) =>
        repo.RemoveItemAsync(itemId);

    public Task<bool> DeleteCartAsync(int cartId) =>
        repo.DeleteCartAsync(cartId);

    private static CartRead ToViewModel(Cart c) => new()
    {
        cart_id = c.CartId,
        user_id = c.UserId,
        items   = c.Items.Select(i => new ItemCartRead
        {
            product_id = i.ProductId,
            quantity   = i.Quantity
        }).ToList()
    };
}
