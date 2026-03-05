using CleanMvcApp.Models.Entities;

namespace CleanMvcApp.Services
{
    public interface INotificationService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendOrderConfirmationAsync(Order order);
        Task SendOrderStatusUpdateAsync(Order order);
        Task SendStoreApprovalAsync(Store store, bool isApproved);
        Task SendLowStockAlertAsync(Product product);
        Task<IEnumerable<Notification>> GetPendingNotificationsAsync();
        Task RetryFailedNotificationsAsync();
    }
}
