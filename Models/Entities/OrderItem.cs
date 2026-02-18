namespace CleanMvcApp.Models.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercentage { get; set; } = 0;
        public decimal LineTotal { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
