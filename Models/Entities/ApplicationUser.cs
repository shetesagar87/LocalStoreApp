using Microsoft.AspNetCore.Identity;

namespace CleanMvcApp.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<Store> Stores { get; set; } = new List<Store>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
