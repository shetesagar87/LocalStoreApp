namespace CleanMvcApp.Services
{
    public interface IAdminDashboardService
    {
        Task<decimal> GetTotalSalesAsync();
        Task<int> GetActiveStoresCountAsync();
        Task<int> GetTotalOrdersCountAsync();
        Task<IEnumerable<(string ProductName, int TotalQuantity)>> GetTopSellingProductsAsync(int count = 10);
        Task<IEnumerable<(string Month, decimal Revenue)>> GetMonthlyRevenueTrendsAsync(int months = 12);
    }
}
