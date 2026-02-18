using CleanMvcApp.Data;
using CleanMvcApp.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanMvcApp.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IRepository<Store>? _stores;
        private IRepository<Product>? _products;
        private IRepository<Category>? _categories;
        private IRepository<Order>? _orders;
        private IRepository<OrderItem>? _orderItems;
        private IRepository<Payment>? _payments;
        private IRepository<CartItem>? _cartItems;
        private IRepository<Review>? _reviews;
        private IRepository<Notification>? _notifications;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Store> Stores => 
            _stores ??= new GenericRepository<Store>(_context);

        public IRepository<Product> Products => 
            _products ??= new GenericRepository<Product>(_context);

        public IRepository<Category> Categories => 
            _categories ??= new GenericRepository<Category>(_context);

        public IRepository<Order> Orders => 
            _orders ??= new GenericRepository<Order>(_context);

        public IRepository<OrderItem> OrderItems => 
            _orderItems ??= new GenericRepository<OrderItem>(_context);

        public IRepository<Payment> Payments => 
            _payments ??= new GenericRepository<Payment>(_context);

        public IRepository<CartItem> CartItems => 
            _cartItems ??= new GenericRepository<CartItem>(_context);

        public IRepository<Review> Reviews => 
            _reviews ??= new GenericRepository<Review>(_context);

        public IRepository<Notification> Notifications => 
            _notifications ??= new GenericRepository<Notification>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
