using Microsoft.EntityFrameworkCore;
using MyGraphQLApi.Data;
using FakeStore.View;

namespace MyGraphQLApi.GraphQL;

public class Query
{
    // User
    public IQueryable<User> GetUsers([Service] FakeStoreDbContext db) =>
        db.Users;

    public Task<User?> GetUser(int id, [Service] FakeStoreDbContext db) =>
        db.Users.FirstOrDefaultAsync(u => u.UserId == id);

    public Task<User?> GetUserByUsername(string username, [Service] FakeStoreDbContext db) =>
        db.Users.FirstOrDefaultAsync(u => u.Username == username);

    public Task<User?> GetUserByEmail(string email, [Service] FakeStoreDbContext db) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email);

    // Product
    public IQueryable<Product> GetProducts([Service] FakeStoreDbContext db) =>
        db.Products.Include(p => p.Reviews);

    public Task<Product?> GetProduct(int id, [Service] FakeStoreDbContext db) =>
        db.Products.Include(p => p.Reviews)
                   .FirstOrDefaultAsync(p => p.ProductId == id);

    // Review
    public IQueryable<Review> GetReviews([Service] FakeStoreDbContext db) =>
        db.Reviews;

    public Task<Review?> GetReview(int id, [Service] FakeStoreDbContext db) =>
        db.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);

    // Cart
    public IQueryable<Cart> GetCarts([Service] FakeStoreDbContext db) =>
        db.Carts.Include(c => c.Items);

    public Task<Cart?> GetCart(int id, [Service] FakeStoreDbContext db) =>
        db.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CartId == id);

    public Task<Cart?> GetCartByUser(int userId, [Service] FakeStoreDbContext db) =>
        db.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

    // CartItem
    public IQueryable<CartItem> GetCartItems([Service] FakeStoreDbContext db) =>
        db.CartItems;

    public Task<CartItem?> GetCartItem(int id, [Service] FakeStoreDbContext db) =>
        db.CartItems.FirstOrDefaultAsync(ci => ci.CartItemId == id);

    // Order
    public IQueryable<Order> GetOrders([Service] FakeStoreDbContext db) =>
        db.Orders.Include(o => o.Items);

    public Task<Order?> GetOrder(int id, [Service] FakeStoreDbContext db) =>
        db.Orders.Include(o => o.Items)
                 .FirstOrDefaultAsync(o => o.OrderId == id);

    public IQueryable<Order> GetOrdersByUser(int userId, [Service] FakeStoreDbContext db) =>
        db.Orders.Include(o => o.Items)
                 .Where(o => o.UserId == userId);

    // OrderItem
    public IQueryable<OrderItem> GetOrderItems([Service] FakeStoreDbContext db) =>
        db.OrderItems;

    public Task<OrderItem?> GetOrderItemByOrder(int orderId, int productId, [Service] FakeStoreDbContext db) =>
        db.OrderItems
          .Where(oi => EF.Property<int>(oi, "OrderId") == orderId && oi.ProductId == productId)
          .FirstOrDefaultAsync();
}
