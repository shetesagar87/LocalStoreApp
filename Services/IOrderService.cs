using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.Enums;

namespace CleanMvcApp.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string customerId, int storeId, string deliveryAddress, PaymentMethod paymentMethod);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<Order?> GetOrderByNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<IEnumerable<Order>> GetOrdersByStoreAsync(int storeId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<bool> AcceptOrderAsync(int orderId);
        Task<bool> RejectOrderAsync(int orderId, string reason);
        Task<bool> CancelOrderAsync(int orderId);
        string GenerateOrderNumber();
    }
}
