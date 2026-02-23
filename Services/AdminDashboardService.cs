using CleanMvcApp.Models.Enums;
using CleanMvcApp.Repositories;

namespace CleanMvcApp.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminDashboardService> _logger;

        public AdminDashboardService(
            IUnitOfWork unitOfWork,
            ILogger<AdminDashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<decimal> GetTotalSalesAsync()
        {
            try
            {
                var orders = await _unitOfWork.Orders.FindAsync(o => 
                    o.Status == OrderStatus.Delivered);
                return orders.Sum(o => o.TotalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total sales");
                return 0;
            }
        }

        public async Task<int> GetActiveStoresCountAsync()
        {
            try
            {
                var stores = await _unitOfWork.Stores.FindAsync(s => 
                    s.Status == StoreStatus.Approved && s.IsActive);
                return stores.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active stores count");
                return 0;
            }
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetAllAsync();
                return orders.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total orders count");
                return 0;
            }
        }

        public async Task<IEnumerable<(string ProductName, int TotalQuantity)>> GetTopSellingProductsAsync(int count = 10)
        {
            try
            {
                var orderItems = await _unitOfWork.OrderItems.GetAllAsync();
                var topProducts = orderItems
                    .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                    .Select(g => new
                    {
                        ProductName = g.Key.ProductName,
                        TotalQuantity = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(p => p.TotalQuantity)
                    .Take(count)
                    .Select(p => (p.ProductName, p.TotalQuantity))
                    .ToList();

                return topProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top selling products");
                return new List<(string, int)>();
            }
        }

        public async Task<IEnumerable<(string Month, decimal Revenue)>> GetMonthlyRevenueTrendsAsync(int months = 12)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddMonths(-months);
                var orders = await _unitOfWork.Orders.FindAsync(o => 
                    o.Status == OrderStatus.Delivered && o.CreatedAt >= startDate);

                var monthlyRevenue = orders
                    .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                    .Select(g => new
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Revenue = g.Sum(o => o.TotalAmount)
                    })
                    .OrderBy(m => m.Month)
                    .Select(m => (m.Month, m.Revenue))
                    .ToList();

                return monthlyRevenue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly revenue trends");
                return new List<(string, decimal)>();
            }
        }
    }
}
