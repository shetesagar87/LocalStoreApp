using System.ComponentModel.DataAnnotations;

namespace CleanMvcApp.Models.ViewModels
{
    public class CreateStoreViewModel
    {
        [Required(ErrorMessage = "Store name is required")]
        [StringLength(200, ErrorMessage = "Store name cannot exceed 200 characters")]
        [Display(Name = "Store Name")]
        public string StoreName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Store Address")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        [Display(Name = "Latitude")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        [Display(Name = "Longitude")]
        public decimal Longitude { get; set; }

        [Required(ErrorMessage = "License number is required")]
        [StringLength(100, ErrorMessage = "License number cannot exceed 100 characters")]
        [Display(Name = "Business License Number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Delivery radius is required")]
        [Range(0.1, 100, ErrorMessage = "Delivery radius must be between 0.1 and 100 km")]
        [Display(Name = "Delivery Radius (km)")]
        public decimal DeliveryRadius { get; set; }

        [Range(0, 10000, ErrorMessage = "Delivery charge must be between 0 and 10000")]
        [Display(Name = "Delivery Charge")]
        public decimal DeliveryCharge { get; set; }

        [Display(Name = "Store Image")]
        public IFormFile? StoreImage { get; set; }
    }
}
