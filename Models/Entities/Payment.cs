using CleanMvcApp.Models.Enums;

namespace CleanMvcApp.Models.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? TransactionId { get; set; }
        public string? PaymentGatewayResponse { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Order Order { get; set; } = null!;
    }
}
