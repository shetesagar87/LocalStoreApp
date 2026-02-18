namespace CleanMvcApp.Models.Entities
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public int? StoreId { get; set; }
        public int? ProductId { get; set; }
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser Customer { get; set; } = null!;
        public Store? Store { get; set; }
        public Product? Product { get; set; }
        public Order Order { get; set; } = null!;
    }
}
