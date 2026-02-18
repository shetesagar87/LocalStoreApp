using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanMvcApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Store> Stores { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(500).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        // Configure Store
        builder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.StoreId);
            entity.Property(e => e.StoreName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Address).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(11, 8)");
            entity.Property(e => e.LicenseNumber).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DeliveryRadius).HasColumnType("decimal(5, 2)").IsRequired();
            entity.Property(e => e.DeliveryCharge).HasColumnType("decimal(10, 2)").HasDefaultValue(0);
            entity.Property(e => e.StoreImageUrl).HasMaxLength(500);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasDefaultValue(StoreStatus.Pending);
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.AverageRating).HasColumnType("decimal(3, 2)").HasDefaultValue(0);
            entity.Property(e => e.TotalReviews).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Owner)
                .WithMany(u => u.Stores)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.OwnerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.Latitude, e.Longitude });
        });

        // Configure Category
        builder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasIndex(e => e.CategoryName).IsUnique();
        });

        // Configure Product
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.SKU).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)").HasDefaultValue(0);
            entity.Property(e => e.StockQuantity).HasDefaultValue(0);
            entity.Property(e => e.ProductImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.AverageRating).HasColumnType("decimal(3, 2)").HasDefaultValue(0);
            entity.Property(e => e.TotalReviews).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Store)
                .WithMany(s => s.Products)
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.StoreId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.ProductName);
            entity.HasIndex(e => e.Price);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.StoreId, e.SKU }).IsUnique();
        });

        // Configure Order
        builder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.OrderNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DeliveryAddress).HasMaxLength(500).IsRequired();
            entity.Property(e => e.SubTotal).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.DeliveryCharge).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasDefaultValue(OrderStatus.Pending);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Customer)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Store)
                .WithMany(s => s.Orders)
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.StoreId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure OrderItem
        builder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId);
            entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)").HasDefaultValue(0);
            entity.Property(e => e.LineTotal).HasColumnType("decimal(10, 2)").IsRequired();

            entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
        });

        // Configure Payment
        builder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.PaymentMethod).HasConversion<string>().HasMaxLength(50).IsRequired();
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)").IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasDefaultValue(PaymentStatus.Pending);
            entity.Property(e => e.TransactionId).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.OrderId).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TransactionId);
        });

        // Configure CartItem
        builder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Customer)
                .WithMany(u => u.CartItems)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.CustomerId, e.ProductId }).IsUnique();
        });

        // Configure Review
        builder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId);
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.IsApproved).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.Customer)
                .WithMany(u => u.Reviews)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Store)
                .WithMany(s => s.Reviews)
                .HasForeignKey(e => e.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Order)
                .WithMany(o => o.Reviews)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.StoreId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.OrderId);

            // Check constraint: Either StoreId or ProductId must be set, but not both
            entity.ToTable(t => t.HasCheckConstraint("CHK_Review_Target",
                "(StoreId IS NOT NULL AND ProductId IS NULL) OR (StoreId IS NULL AND ProductId IS NOT NULL)"));

            // Check constraint: Rating must be between 1 and 5
            entity.ToTable(t => t.HasCheckConstraint("CHK_Review_Rating", "Rating >= 1 AND Rating <= 5"));
        });

        // Configure Notification
        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);
            entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Subject).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Body).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(50).HasDefaultValue(NotificationStatus.Pending);
            entity.Property(e => e.RetryCount).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
