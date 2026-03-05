using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.Enums;
using CleanMvcApp.Repositories;

namespace CleanMvcApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IUnitOfWork unitOfWork,
            ICartService cartService,
            INotificationService notificationService,
            ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(string customerId, int storeId, string deliveryAddress, PaymentMethod paymentMethod)
        {
            try
            {
                // Get cart items for this customer and store
                var allCartItems = await _cartService.GetCartAsync(customerId);
                var cartItems = allCartItems.Where(c => c.Product.StoreId == storeId).ToList();

                if (!cartItems.Any())
                {
                    throw new InvalidOperationException("No items in cart for this store.");
                }

                // Validate stock for all items
                foreach (var cartItem in cartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                    if (product == null)
                    {
                        throw new InvalidOperationException($"Product {cartItem.ProductId} not found.");
                    }

                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for {product.ProductName}. Only {product.StockQuantity} available.");
                    }
                }

                // Get store for delivery charge
                var store = await _unitOfWork.Stores.GetByIdAsync(storeId);
                if (store == null)
                {
                    throw new InvalidOperationException("Store not found.");
                }

                // Calculate totals
                decimal subtotal = 0;
                var orderItems = new List<OrderItem>();

                foreach (var cartItem in cartItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                    if (product != null)
                    {
                        var unitPrice = product.Price;
                        var discountPercentage = product.DiscountPercentage;
                        var lineTotal = unitPrice * cartItem.Quantity * (1 - discountPercentage / 100);

                        orderItems.Add(new OrderItem
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            Quantity = cartItem.Quantity,
                            UnitPrice = unitPrice,
                            DiscountPercentage = discountPercentage,
                            LineTotal = lineTotal
                        });

                        subtotal += lineTotal;
                    }
                }

                // Create order
                var order = new Order
                {
                    CustomerId = customerId,
                    StoreId = storeId,
                    OrderNumber = GenerateOrderNumber(),
                    DeliveryAddress = deliveryAddress,
                    SubTotal = subtotal,
                    DeliveryCharge = store.DeliveryCharge,
                    TotalAmount = subtotal + store.DeliveryCharge,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderItems = orderItems
                };

                // Save order
                var createdOrder = await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                // Reduce stock for each product
                foreach (var orderItem in orderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= orderItem.Quantity;
                        product.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.Products.UpdateAsync(product);
                    }
                }

                // Create payment record
                var payment = new Payment
                {
                    OrderId = createdOrder.OrderId,
                    Amount = order.TotalAmount,
                    PaymentMethod = paymentMethod,
                    Status = paymentMethod == PaymentMethod.CashOnDelivery 
                        ? PaymentStatus.Pending 
                        : PaymentStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                // Clear cart items for this store
                foreach (var cartItem in cartItems)
                {
                    await _cartService.RemoveFromCartAsync(cartItem.CartItemId);
                }

                _logger.LogInformation("Order {OrderNumber} created for customer {CustomerId}", order.OrderNumber, customerId);
                
                // Send order confirmation email
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.SendOrderConfirmationAsync(createdOrder);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send order confirmation for {OrderNumber}", order.OrderNumber);
                    }
                });

                return createdOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.OrderId == orderId);
            return orders.FirstOrDefault();
        }

        public async Task<Order?> GetOrderByNumberAsync(string orderNumber)
        {
            var orders = await _unitOfWork.Orders.FindAsync(o => o.OrderNumber == orderNumber);
            return orders.FirstOrDefault();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _unitOfWork.Orders.FindAsync(o => o.CustomerId == customerId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStoreAsync(int storeId)
        {
            return await _unitOfWork.Orders.FindAsync(o => o.StoreId == storeId);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", orderId);
                    return false;
                }

                // Validate state transitions
                if (!IsValidStatusTransition(order.Status, newStatus))
                {
                    throw new InvalidOperationException($"Invalid status transition from {order.Status} to {newStatus}");
                }

                order.Status = newStatus;
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} status updated to {Status}", orderId, newStatus);
                
                // Send status update notification
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.SendOrderStatusUpdateAsync(order);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send status update for Order {OrderId}", orderId);
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status", orderId);
                throw;
            }
        }

        public async Task<bool> AcceptOrderAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return false;
                }

                if (order.Status != OrderStatus.Pending)
                {
                    throw new InvalidOperationException("Only pending orders can be accepted.");
                }

                order.Status = OrderStatus.Accepted;
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} accepted", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> RejectOrderAsync(int orderId, string reason)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return false;
                }

                if (order.Status != OrderStatus.Pending)
                {
                    throw new InvalidOperationException("Only pending orders can be rejected.");
                }

                order.Status = OrderStatus.Cancelled;
                order.RejectionReason = reason;
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Orders.UpdateAsync(order);

                // Restore stock
                var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == orderId);
                foreach (var orderItem in orderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += orderItem.Quantity;
                        product.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.Products.UpdateAsync(product);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} rejected: {Reason}", orderId, reason);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return false;
                }

                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Accepted)
                {
                    throw new InvalidOperationException("Only pending or accepted orders can be cancelled.");
                }

                order.Status = OrderStatus.Cancelled;
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Orders.UpdateAsync(order);

                // Restore stock
                var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == orderId);
                foreach (var orderItem in orderItems)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(orderItem.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += orderItem.Quantity;
                        product.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.Products.UpdateAsync(product);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} cancelled", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", orderId);
                throw;
            }
        }

        public string GenerateOrderNumber()
        {
            // Format: ORD-YYYYMMDD-HHMMSS-RANDOM
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp}-{random}";
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Define valid state transitions
            return currentStatus switch
            {
                OrderStatus.Pending => newStatus == OrderStatus.Accepted || newStatus == OrderStatus.Cancelled,
                OrderStatus.Accepted => newStatus == OrderStatus.Preparing || newStatus == OrderStatus.Cancelled,
                OrderStatus.Preparing => newStatus == OrderStatus.OutForDelivery || newStatus == OrderStatus.Cancelled,
                OrderStatus.OutForDelivery => newStatus == OrderStatus.Delivered,
                OrderStatus.Delivered => newStatus == OrderStatus.Refunded,
                OrderStatus.Cancelled => false,
                OrderStatus.Refunded => false,
                _ => false
            };
        }
    }
}
