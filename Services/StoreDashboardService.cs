using CleanMvcApp.Models.Enums;
using CleanMvcApp.Repositories;

namespace CleanMvcApp.Services
{
    public class StoreDashboardService : IStoreDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StoreDashboardService> _logger;

        public StoreDashboardService(
            IUnitOfWork unitOfWork,
            ILogger<StoreDashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<decimal> GetDailySalesAsync(int storeId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var orders = await _unitOfWork.Orders.FindAsync(o => 
                    o.StoreId == storeId && 
                    o.CreatedAt.Date == today &&
                    o.Status == OrderStatus.Delivered);
                
                return orders.Sum(o => o.TotalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily sales for store {StoreId}", storeId);
                return 0;
            }
        }

        public async Task<int> GetPendingOrdersCountAsync(int storeId)
        {
            try
            {
                var orders = await _unitOfWork.Orders.FindAsync(o => 
                    o.StoreId == storeId && 
                    o.Status == OrderStatus.Pending);
                
                return orders.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending orders count for store {StoreId}", storeId);
                return 0;
            }
        }

        public async Task<IEnumerable<(string ProductName, int StockQuantity)>> GetLowStockProductsAsync(int storeId, int threshold = 10)
        {
            try
            {
                var products = await _unitOfWork.Products.FindAsync(p => 
                    p.StoreId == storeId && 
                    p.StockQuantity <= threshold &&
                    !p.IsDeleted);
                
                return products
                    .OrderBy(p => p.StockQuantity)
                    .Select(p => (p.ProductName, p.StockQuantity))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock products for store {StoreId}", storeId);
                return new List<(string, int)>();
            }
        }

        public async Task<(decimal TotalRevenue, int TotalOrders, decimal AverageOrderValue)> GetMonthlyStatsAsync(int storeId)
        {
            try
            {
                var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var orders = await _unitOfWork.Orders.FindAsync(o => 
                    o.StoreId == storeId && 
                    o.CreatedAt >= startOfMonth &&
                    o.Status == OrderStatus.Delivered);
                
                var totalRevenue = orders.Sum(o => o.TotalAmount);
                var totalOrders = orders.Count();
                var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

                return (totalRevenue, totalOrders, averageOrderValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting monthly stats for store {StoreId}", storeId);
                return (0, 0, 0);
            }
        }
    }
}
