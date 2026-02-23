namespace CleanMvcApp.Services
{
    public interface IStoreDashboardService
    {
        Task<decimal> GetDailySalesAsync(int storeId);
        Task<int> GetPendingOrdersCountAsync(int storeId);
        Task<IEnumerable<(string ProductName, int StockQuantity)>> GetLowStockProductsAsync(int storeId, int threshold = 10);
        Task<(decimal TotalRevenue, int TotalOrders, decimal AverageOrderValue)> GetMonthlyStatsAsync(int storeId);
    }
}
