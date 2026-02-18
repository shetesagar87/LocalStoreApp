using CleanMvcApp.Models.Enums;

namespace CleanMvcApp.Models.Entities
{
    public class Store
    {
        public int StoreId { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public decimal DeliveryRadius { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public decimal DeliveryCharge { get; set; } = 0;
        public string? StoreImageUrl { get; set; }
        public StoreStatus Status { get; set; } = StoreStatus.Pending;
        public bool IsActive { get; set; } = false;
        public decimal AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser Owner { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
