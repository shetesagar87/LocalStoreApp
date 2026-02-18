namespace CleanMvcApp.Models.Entities
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser Customer { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
