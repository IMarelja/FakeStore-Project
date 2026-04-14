using Microsoft.EntityFrameworkCore;
using MyGraphQLApi.Data;
using FakeStore.Models;

namespace MyGraphQLApi.GraphQL;

public class Mutation
{
    // ── User ───────────────────────────────────────────────────────────────

    public async Task<User> CreateUser(UserInput input, [Service] FakeStoreDbContext db)
    {
        var user = new User
        {
            Username = input.Username,
            Email = input.Email,
            Password = input.Password,
            Role = input.Role ?? "read-only"
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> UpdateUser(int id, UserInput input, [Service] FakeStoreDbContext db)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return null;
        user.Username = input.Username;
        user.Email = input.Email;
        user.Password = input.Password;
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUser(int id, [Service] FakeStoreDbContext db)
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return false;
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Product ────────────────────────────────────────────────────────────

    public async Task<Product> CreateProduct(ProductInput input, [Service] FakeStoreDbContext db)
    {
        var product = new Product
        {
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            Unit = input.Unit ?? string.Empty,
            Image = input.Image ?? string.Empty,
            Discount = input.Discount,
            Available = input.Available,
            Brand = input.Brand,
            Category = input.Category,
            Rating = 0 // Always zero
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateProduct(int id, ProductInput input, [Service] FakeStoreDbContext db)
    {
        var product = await db.Products.FindAsync(id);

        if (product is null) 
            return null;
        
        product.Name = input.Name;
        product.Description = input.Description;
        product.Price = input.Price;
        product.Unit = input.Unit ?? product.Unit;
        product.Image = input.Image;
        product.Discount = input.Discount;
        product.Available = input.Available;
        product.Brand = input.Brand;
        product.Category = input.Category;

        // Review must stay unchanged
        product.Rating = product.Rating;

        await db.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProduct(int id, [Service] FakeStoreDbContext db)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return false;
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Review ─────────────────────────────────────────────────────────────

    public async Task<Review> CreateReview(ReviewInput input, [Service] FakeStoreDbContext db)
    {
        var review = new Review
        {
            UserId = input.UserId,
            ProductId = input.ProductId,
            Rating = input.Rating,
            Comment = input.Comment
        };
        db.Reviews.Add(review);
        await db.SaveChangesAsync();

        await RecalculateProductRatingAsync(review.ProductId, db);

        return review;
    }

    public async Task<Review?> UpdateReview(int id, ReviewInput input, [Service] FakeStoreDbContext db)
    {
        var review = await db.Reviews.FindAsync(id);
        if (review is null) 
            return null;

        review.UserId = input.UserId;
        review.Rating = input.Rating;
        review.Comment = input.Comment;

        // Products review must stay the same
        review.ProductId = review.ProductId;

        await db.SaveChangesAsync();

        await RecalculateProductRatingAsync(review.ProductId, db);

        return review;
    }

    public async Task<bool> DeleteReview(int id, [Service] FakeStoreDbContext db)
    {
        var review = await db.Reviews.FindAsync(id);
        if (review is null) 
            return false;

        var productId = review.ProductId;
        db.Reviews.Remove(review);
        await db.SaveChangesAsync();

        await RecalculateProductRatingAsync(productId, db);

        return true;
    }

    private static async Task RecalculateProductRatingAsync(int productId, FakeStoreDbContext db)
    {
        var product = await db.Products.FindAsync(productId);
        if (product is null) return;

        product.Rating = await db.Reviews
                                 .Where(r => r.ProductId == productId)
                                 .Select(r => (double?)r.Rating)
                                 .AverageAsync() ?? 0;

        await db.SaveChangesAsync();
    }

    // ── Cart ───────────────────────────────────────────────────────────────

    public async Task<Cart> CreateCart(CartInput input, [Service] FakeStoreDbContext db)
    {
        var cart = new Cart { UserId = input.UserId };
        db.Carts.Add(cart);
        await db.SaveChangesAsync();
        return cart;
    }

    public async Task<Cart?> UpdateCart(int id, CartInput input, [Service] FakeStoreDbContext db)
    {
        var cart = await db.Carts.FindAsync(id);
        if (cart is null) return null;
        cart.UserId = input.UserId;
        await db.SaveChangesAsync();
        return cart;
    }

    public async Task<bool> DeleteCart(int id, [Service] FakeStoreDbContext db)
    {
        var cart = await db.Carts.FindAsync(id);
        if (cart is null) return false;
        db.Carts.Remove(cart);
        await db.SaveChangesAsync();
        return true;
    }

    // ── CartItem ───────────────────────────────────────────────────────────

    public async Task<CartItem> CreateCartItem(CartItemInput input, [Service] FakeStoreDbContext db)
    {

        var existingItem = await db.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == input.CartId && ci.ProductId == input.ProductId);

        if (existingItem is not null)
        {
            existingItem.Quantity += input.Quantity;
            await db.SaveChangesAsync();
            return existingItem;
        }

        var item = new CartItem
        {
            CartId = input.CartId,
            ProductId = input.ProductId,
            Quantity = input.Quantity
        };
        db.CartItems.Add(item);
        await db.SaveChangesAsync();
        return item;
    }

    public async Task<CartItem?> UpdateCartItem(int id, CartItemInput input, [Service] FakeStoreDbContext db)
    {
        var item = await db.CartItems.FindAsync(id);
        if (item is null) 
            return null;

        if (input.Quantity == 0)
        {
            db.CartItems.Remove(item);
            await db.SaveChangesAsync();
            return item;
        }

        item.Quantity = input.Quantity;
        await db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteCartItem(int id, [Service] FakeStoreDbContext db)
    {
        var item = await db.CartItems.FindAsync(id);
        if (item is null) return false;
        db.CartItems.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }

    // ── Order ──────────────────────────────────────────────────────────────

    public async Task<Order> CreateOrder(OrderInput input, [Service] FakeStoreDbContext db)
    {
        var userExists = await db.Users.AnyAsync(u => u.UserId == input.UserId);
        if (!userExists)
            throw new Exception($"User with ID {input.UserId} does not exist.");

        var order = new Order
        {
            UserId = input.UserId,
            OrderStatus = input.OrderStatus,
            TotalPrice = input.TotalPrice
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync(); // order.OrderId is populated here

        decimal totalPrice = 0;
        foreach (var item in input.Items)
        {
            var product = await db.Products.FindAsync(item.ProductId);
            if (product is null) continue;

            var discountedPrice = product.Price * (1 - product.Discount / 100m);
            totalPrice += discountedPrice * item.Quantity;

            var orderItem = new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity };
            db.OrderItems.Add(orderItem);
            db.Entry(orderItem).Property("OrderId").CurrentValue = order.OrderId;
        }

        order.TotalPrice = totalPrice;
        await db.SaveChangesAsync();
        return order;
        
    }

    public async Task<Order?> UpdateOrder(int id, OrderUpdateInput input, [Service] FakeStoreDbContext db)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null) 
            return null;

        if (input.UserId.HasValue)
        {
            order.UserId = input.UserId.Value;
        }    
            
        if (input.OrderStatus is not null)
        {
            order.OrderStatus = input.OrderStatus;
        }
            
        if (input.TotalPrice.HasValue)
        {
            order.TotalPrice = input.TotalPrice.Value;
        }
            
        await db.SaveChangesAsync();
        return order;
    }

    public async Task<bool> DeleteOrder(int id, [Service] FakeStoreDbContext db)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null) return false;
        db.Orders.Remove(order);
        await db.SaveChangesAsync();
        return true;
    }

    // ── OrderItem ──────────────────────────────────────────────────────────

    public async Task<OrderItem> CreateOrderItem(OrderItemInput input, [Service] FakeStoreDbContext db)
    {
        var item = new OrderItem { ProductId = input.ProductId, Quantity = input.Quantity };
        db.OrderItems.Add(item);
        // Set the shadow FK before saving
        db.Entry(item).Property("OrderId").CurrentValue = input.OrderId;
        await db.SaveChangesAsync();
        return item;
    }

    public async Task<OrderItem?> UpdateOrderItem(int orderId, int productId, int quantity, [Service] FakeStoreDbContext db)
    {
        var item = await db.OrderItems
            .Where(oi => EF.Property<int>(oi, "OrderId") == orderId && oi.ProductId == productId)
            .FirstOrDefaultAsync();
        if (item is null) return null;
        item.Quantity = quantity;
        await db.SaveChangesAsync();
        return item;
    }

    public async Task<bool> DeleteOrderItem(int orderId, int productId, [Service] FakeStoreDbContext db)
    {
        var item = await db.OrderItems
            .Where(oi => EF.Property<int>(oi, "OrderId") == orderId && oi.ProductId == productId)
            .FirstOrDefaultAsync();
        if (item is null) return false;
        db.OrderItems.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }
}
