using CleanMvcApp.Models.Entities;

namespace CleanMvcApp.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Store> Stores { get; }
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<Payment> Payments { get; }
        IRepository<CartItem> CartItems { get; }
        IRepository<Review> Reviews { get; }
        IRepository<Notification> Notifications { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
