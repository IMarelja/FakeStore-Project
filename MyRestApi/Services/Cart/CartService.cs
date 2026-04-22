using FakeStore.Models;
using MyRestApi.DTO.Cart;
using MyRestApi.Repositories;

namespace MyRestApi.Services;

public class CartService(ICartRepo repo) : ICartService
{
    public async Task<List<CartReadDto>> GetAllAsync()
    {
        var carts = await repo.GetAllAsync();
        return carts.Select(ToDto).ToList();
    }

    public async Task<CartReadDto?> GetByIdAsync(int cartId)
    {
        var cart = await repo.GetByIdAsync(cartId);
        return cart is null ? null : ToDto(cart);
    }

    public async Task<CartReadDto?> GetByUserIdAsync(int userId)
    {
        var cart = await repo.GetByUserIdAsync(userId);
        return cart is null ? null : ToDto(cart);
    }

    public async Task<CartReadDto?> AddItemAsync(int cartId, CartItemAddDto req)
    {
        var cart = await repo.AddItemAsync(cartId, req);
        return cart is null ? null : ToDto(cart);
    }

    public async Task<CartReadDto?> EditItemAsync(int cartId, int productId, CartItemEditDto req)
    {
        var cart = await repo.EditItemAsync(cartId, productId, req);
        return cart is null ? null : ToDto(cart);
    }

    public Task<bool> RemoveItemAsync(int itemId) =>
        repo.RemoveItemAsync(itemId);

    public Task<bool> RemoveItemByUserAndProductAsync(int userId, int productId) =>
        repo.RemoveItemByUserAndProductAsync(userId, productId);

    public Task<bool> DeleteCartAsync(int cartId) =>
        repo.DeleteCartAsync(cartId);

    private static CartReadDto ToDto(Cart c) => new()
    {
        cart_id = c.CartId,
        user_id = c.UserId,
        items   = c.Items.Select(i => new CartItemReadDto
        {
            cart_item_id = i.CartItemId,
            product_id = i.ProductId,
            quantity   = i.Quantity
        }).ToList()
    };
}
