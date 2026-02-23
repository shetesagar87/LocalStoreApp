using System.ComponentModel.DataAnnotations;

namespace CleanMvcApp.Models.ViewModels
{
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [StringLength(100, ErrorMessage = "SKU cannot exceed 100 characters")]
        [Display(Name = "SKU")]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        [Display(Name = "Discount (%)")]
        public decimal DiscountPercentage { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, 1000000, ErrorMessage = "Stock quantity must be between 0 and 1,000,000")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Product Image")]
        public IFormFile? ProductImage { get; set; }
    }
}
