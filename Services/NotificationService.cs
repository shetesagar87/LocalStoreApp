using CleanMvcApp.Data;
using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.Enums;
using CleanMvcApp.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace CleanMvcApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var notification = new Notification
            {
                UserId = "", // Will be set if user is found
                Type = "Email",
                Subject = subject,
                Body = body,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                // Get SMTP settings from configuration
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];

                // Check if SMTP is configured
                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(fromEmail))
                {
                    _logger.LogWarning("SMTP not configured. Email notification logged but not sent.");
                    notification.Status = NotificationStatus.Failed;
                    await _unitOfWork.Notifications.AddAsync(notification);
                    await _unitOfWork.SaveChangesAsync();
                    return;
                }

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }

                notification.Status = NotificationStatus.Sent;
                notification.SentAt = DateTime.UtcNow;
                _logger.LogInformation($"Email sent successfully to {toEmail}");
            }
            catch (Exception ex)
            {
                notification.Status = NotificationStatus.Failed;
                _logger.LogError(ex, $"Failed to send email to {toEmail}");
            }

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SendOrderConfirmationAsync(Order order)
        {
            var customers = await _unitOfWork.Orders.FindAsync(o => o.OrderId == order.OrderId);
            var orderWithCustomer = customers.FirstOrDefault();
            
            if (orderWithCustomer == null)
            {
                _logger.LogWarning($"Cannot send order confirmation: Order {order.OrderNumber} not found");
                return;
            }

            var store = await _unitOfWork.Stores.GetByIdAsync(order.StoreId);
            if (store == null)
            {
                _logger.LogWarning($"Cannot send order confirmation: Store not found for Order {order.OrderNumber}");
                return;
            }

            // Get customer email - need to query separately
            var allUsers = await _unitOfWork.Orders.FindAsync(o => o.CustomerId == order.CustomerId);
            var customerEmail = order.CustomerId; // Fallback to ID

            var subject = $"Order Confirmation - {order.OrderNumber}";
            var body = $@"
                <h2>Order Confirmation</h2>
                <p>Thank you for your order! Your order has been placed successfully.</p>
                <h3>Order Details:</h3>
                <ul>
                    <li><strong>Order Number:</strong> {order.OrderNumber}</li>
                    <li><strong>Store:</strong> {store.StoreName}</li>
                    <li><strong>Total Amount:</strong> ${order.TotalAmount:F2}</li>
                    <li><strong>Delivery Address:</strong> {order.DeliveryAddress}</li>
                    <li><strong>Status:</strong> {order.Status}</li>
                </ul>
                <p>You will receive updates as your order progresses.</p>
                <p>Thank you for shopping with us!</p>
            ";

            // Create notification record
            var notification = new Notification
            {
                UserId = order.CustomerId,
                Type = "OrderConfirmation",
                Subject = subject,
                Body = body,
                Status = NotificationStatus.Sent,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation($"Order confirmation notification created for {order.OrderNumber}");
        }

        public async Task SendOrderStatusUpdateAsync(Order order)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(order.StoreId);
            if (store == null)
            {
                _logger.LogWarning($"Cannot send status update: Store not found for Order {order.OrderNumber}");
                return;
            }

            var subject = $"Order Status Update - {order.OrderNumber}";
            var body = $@"
                <h2>Order Status Update</h2>
                <p>Your order status has been updated.</p>
                <h3>Order Details:</h3>
                <ul>
                    <li><strong>Order Number:</strong> {order.OrderNumber}</li>
                    <li><strong>Store:</strong> {store.StoreName}</li>
                    <li><strong>New Status:</strong> {order.Status}</li>
                    <li><strong>Updated At:</strong> {order.UpdatedAt:g}</li>
                </ul>
                <p>Thank you for your patience!</p>
            ";

            // Create notification record
            var notification = new Notification
            {
                UserId = order.CustomerId,
                Type = "OrderStatusUpdate",
                Subject = subject,
                Body = body,
                Status = NotificationStatus.Sent,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation($"Order status update notification created for {order.OrderNumber}");
        }

        public async Task SendStoreApprovalAsync(Store store, bool isApproved)
        {
            var subject = isApproved 
                ? $"Store Approved - {store.StoreName}" 
                : $"Store Registration Update - {store.StoreName}";
            
            var body = isApproved
                ? $@"
                    <h2>Congratulations!</h2>
                    <p>Your store <strong>{store.StoreName}</strong> has been approved!</p>
                    <p>You can now start adding products and accepting orders.</p>
                    <p>Store Details:</p>
                    <ul>
                        <li><strong>Store Name:</strong> {store.StoreName}</li>
                        <li><strong>Address:</strong> {store.Address}</li>
                        <li><strong>License Number:</strong> {store.LicenseNumber}</li>
                    </ul>
                    <p>Welcome to the Local Store Platform!</p>
                "
                : $@"
                    <h2>Store Registration Update</h2>
                    <p>We regret to inform you that your store <strong>{store.StoreName}</strong> registration could not be approved at this time.</p>
                    <p>Please contact support for more information.</p>
                ";

            // Create notification record
            var notification = new Notification
            {
                UserId = store.OwnerId,
                Type = isApproved ? "StoreApproved" : "StoreRejected",
                Subject = subject,
                Body = body,
                Status = NotificationStatus.Sent,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation($"Store approval notification created for {store.StoreName}");
        }

        public async Task SendLowStockAlertAsync(Product product)
        {
            var store = await _unitOfWork.Stores.GetByIdAsync(product.StoreId);
            if (store == null)
            {
                _logger.LogWarning($"Cannot send low stock alert: Store not found for Product {product.ProductName}");
                return;
            }

            var subject = $"Low Stock Alert - {product.ProductName}";
            var body = $@"
                <h2>Low Stock Alert</h2>
                <p>The following product in your store <strong>{store.StoreName}</strong> is running low on stock:</p>
                <ul>
                    <li><strong>Product:</strong> {product.ProductName}</li>
                    <li><strong>SKU:</strong> {product.SKU}</li>
                    <li><strong>Current Stock:</strong> {product.StockQuantity}</li>
                </ul>
                <p>Please restock this item to avoid running out.</p>
            ";

            // Create notification record
            var notification = new Notification
            {
                UserId = store.OwnerId,
                Type = "LowStockAlert",
                Subject = subject,
                Body = body,
                Status = NotificationStatus.Sent,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation($"Low stock alert notification created for Product {product.ProductName}");
        }

        public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync()
        {
            return await _unitOfWork.Notifications.FindAsync(n => n.Status == NotificationStatus.Pending);
        }

        public async Task RetryFailedNotificationsAsync()
        {
            var failedNotifications = await _unitOfWork.Notifications
                .FindAsync(n => n.Status == NotificationStatus.Failed && n.RetryCount < 3);

            foreach (var notification in failedNotifications)
            {
                notification.RetryCount++;
                notification.Status = NotificationStatus.Pending;
                await _unitOfWork.Notifications.UpdateAsync(notification);
            }
            
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"Marked {failedNotifications.Count()} failed notifications for retry");
        }
    }
}
