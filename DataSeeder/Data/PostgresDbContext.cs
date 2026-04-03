using Microsoft.EntityFrameworkCore;
using FakeStore.View;

namespace DataSeeder.Data;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) { }

    public DbSet<User>      Users      => Set<User>();
    public DbSet<Product>   Products   => Set<Product>();
    public DbSet<Review>    Reviews    => Set<Review>();
    public DbSet<Cart>      Carts      => Set<Cart>();
    public DbSet<CartItem>  CartItems  => Set<CartItem>();
    public DbSet<Order>     Orders     => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.UserId);
            e.Property(u => u.UserId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("product");
            e.HasKey(p => p.ProductId);
            e.Property(p => p.ProductId).ValueGeneratedNever();
            e.Ignore(p => p.Reviews);
        });

        modelBuilder.Entity<Review>(e =>
        {
            e.ToTable("review");
            e.HasKey(r => r.ReviewId);
            e.Property(r => r.ReviewId).ValueGeneratedOnAdd();
            e.HasOne<Product>().WithMany().HasForeignKey(r => r.ProductId);
            e.HasOne<User>().WithMany().HasForeignKey(r => r.UserId);
        });

        modelBuilder.Entity<Cart>(e =>
        {
            e.ToTable("cart");
            e.HasKey(c => c.CartId);
            e.Property(c => c.CartId).ValueGeneratedNever();
            e.Ignore(c => c.Items);
        });

        modelBuilder.Entity<CartItem>(e =>
        {
            e.ToTable("cart_item");
            e.HasKey(ci => ci.CartItemId);
            e.Property(ci => ci.CartItemId).ValueGeneratedOnAdd();
            e.HasOne<Cart>().WithMany().HasForeignKey(ci => ci.CartId);
            e.HasOne<Product>().WithMany().HasForeignKey(ci => ci.ProductId);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("orders");
            e.HasKey(o => o.OrderId);
            e.Property(o => o.OrderId).ValueGeneratedNever();
            e.Ignore(o => o.Items);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_item");
            entity.Property<int>("OrderItemId").ValueGeneratedOnAdd();
            entity.HasKey("OrderItemId");
            entity.Property<int>("OrderId");
            entity.HasOne<Order>().WithMany().HasForeignKey("OrderId");
            entity.HasOne<Product>().WithMany().HasForeignKey(oi => oi.ProductId);
        });
    }
}
