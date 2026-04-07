using Microsoft.EntityFrameworkCore;
using FakeStore.Models;

namespace MyGraphQLApi.Data;

public class FakeStoreDbContext : DbContext
{
    public FakeStoreDbContext(DbContextOptions<FakeStoreDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Product>().ToTable("product");
        modelBuilder.Entity<Review>().ToTable("review");
        modelBuilder.Entity<Cart>().ToTable("cart");
        modelBuilder.Entity<CartItem>().ToTable("cart_item");
        modelBuilder.Entity<Order>().ToTable("orders");

        // OrderItem has no C# PK/FK — configure them as shadow properties
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_item");
            entity.Property<int>("OrderItemId").ValueGeneratedOnAdd();
            entity.HasKey("OrderItemId");
            entity.HasOne<Order>()
                  .WithMany(o => o.Items)
                  .HasForeignKey("OrderId");
        });
    }
}
