using CleanMvcApp.Models.Enums;

namespace CleanMvcApp.Models.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public int StoreId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal SubTotal { get; set; }
        public decimal DeliveryCharge { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser Customer { get; set; } = null!;
        public Store Store { get; set; } = null!;
        public Payment? Payment { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
