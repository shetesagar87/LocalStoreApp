namespace CleanMvcApp.Models.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; } = 0;
        public int StockQuantity { get; set; } = 0;
        public string? ProductImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public decimal AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Store Store { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
