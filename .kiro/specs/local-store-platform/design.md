# Design Document: Local Store Delivery Platform

## Overview

The Local Store Delivery Platform is built using ASP.NET MVC with a layered architecture pattern. The system follows the Model-View-Controller pattern with clear separation between presentation, business logic, and data access layers. The platform uses Entity Framework Core for ORM, ASP.NET Identity for authentication, and SQL Server for data persistence.

The system supports three distinct user roles (Admin, StoreOwner, Customer) with role-based access control enforced at both the controller and service layers. Geo-location calculations use the Haversine formula to determine store proximity. The platform implements a complete e-commerce workflow including cart management, checkout, payment integration, and order lifecycle tracking.

## Architecture

### Layered Architecture

The system follows a 3-tier architecture:

**Presentation Layer (MVC Controllers + Views)**
- Handles HTTP requests and responses
- Renders Razor views with Bootstrap UI
- Implements client-side validation
- Manages user sessions and authentication cookies

**Business Logic Layer (Services)**
- Contains core business rules and validation
- Implements service interfaces for dependency injection
- Handles transaction management
- Performs authorization checks
- Calculates geo-location distances
- Manages order state transitions

**Data Access Layer (Repositories + Entity Framework)**
- Implements repository pattern for data access
- Uses Entity Framework Core DbContext
- Handles database queries and updates
- Implements unit of work pattern
- Manages database transactions

### Technology Stack

- **Framework**: ASP.NET Core MVC 6.0+
- **Language**: C# 10+
- **ORM**: Entity Framework Core 6.0+
- **Database**: SQL Server 2019+
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Razor Views, Bootstrap 5, jQuery
- **Payment**: Stripe/Razorpay SDK integration
- **Email**: SMTP with MailKit
- **Logging**: Serilog with SQL Server sink
- **Caching**: IMemoryCache (in-memory)

## Components and Interfaces

### Core Services

**IAuthenticationService**
```csharp
public interface IAuthenticationService
{
    Task<IdentityResult> RegisterAsync(RegisterViewModel model, string role);
    Task<SignInResult> LoginAsync(LoginViewModel model);
    Task LogoutAsync();
    Task<bool> SendEmailVerificationAsync(string userId);
    Task<IdentityResult> VerifyEmailAsync(string userId, string token);
}
```

**IStoreService**
```csharp
public interface IStoreService
{
    Task<Store> CreateStoreAsync(StoreCreateDto dto, string ownerId);
    Task<Store> UpdateStoreAsync(int storeId, StoreUpdateDto dto);
    Task<bool> ApproveStoreAsync(int storeId, string adminId);
    Task<bool> RejectStoreAsync(int storeId, string adminId, string reason);
    Task<bool> ToggleStoreStatusAsync(int storeId, bool isActive);
    Task<List<Store>> GetNearbyStoresAsync(string customerAddress, double maxDistance);
    Task<Store> GetStoreByIdAsync(int storeId);
}
```

**IProductService**
```csharp
public interface IProductService
{
    Task<Product> CreateProductAsync(ProductCreateDto dto, int storeId);
    Task<Product> UpdateProductAsync(int productId, ProductUpdateDto dto);
    Task<bool> DeleteProductAsync(int productId);
    Task<List<Product>> SearchProductsAsync(ProductSearchDto searchDto, string customerAddress);
    Task<bool> UpdateStockAsync(int productId, int quantity);
    Task<Product> GetProductByIdAsync(int productId);
}
```

**ICartService**
```csharp
public interface ICartService
{
    Task<CartItem> AddToCartAsync(string customerId, int productId, int quantity);
    Task<bool> UpdateCartItemAsync(int cartItemId, int quantity);
    Task<bool> RemoveFromCartAsync(int cartItemId);
    Task<Cart> GetCartAsync(string customerId);
    Task<decimal> GetCartSubtotalAsync(string customerId);
    Task ClearCartAsync(string customerId);
}
```

**IOrderService**
```csharp
public interface IOrderService
{
    Task<Order> CreateOrderAsync(string customerId, CheckoutDto dto);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus, string userId);
    Task<bool> AcceptOrderAsync(int orderId, string storeOwnerId);
    Task<bool> RejectOrderAsync(int orderId, string storeOwnerId, string reason);
    Task<List<Order>> GetOrdersByCustomerAsync(string customerId);
    Task<List<Order>> GetOrdersByStoreAsync(int storeId);
    Task<Order> GetOrderByIdAsync(int orderId);
}
```

**IPaymentService**
```csharp
public interface IPaymentService
{
    Task<PaymentResult> ProcessOnlinePaymentAsync(int orderId, PaymentMethodDto dto);
    Task<bool> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status);
    Task<bool> InitiateRefundAsync(int orderId);
    Task<Payment> GetPaymentByOrderIdAsync(int orderId);
}
```

**IReviewService**
```csharp
public interface IReviewService
{
    Task<Review> CreateStoreReviewAsync(string customerId, int storeId, ReviewCreateDto dto);
    Task<Review> CreateProductReviewAsync(string customerId, int productId, ReviewCreateDto dto);
    Task<bool> DeleteReviewAsync(int reviewId, string adminId);
    Task<double> CalculateStoreRatingAsync(int storeId);
    Task<double> CalculateProductRatingAsync(int productId);
    Task<List<Review>> GetStoreReviewsAsync(int storeId);
}
```

**INotificationService**
```csharp
public interface INotificationService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendOrderConfirmationAsync(int orderId);
    Task SendOrderStatusUpdateAsync(int orderId);
    Task SendStoreApprovalAsync(int storeId, bool approved);
    Task SendLowStockAlertAsync(int productId);
}
```

**IGeoLocationService**
```csharp
public interface IGeoLocationService
{
    double CalculateDistance(string address1, string address2);
    Task<Coordinates> GeocodeAddressAsync(string address);
}
```

### Repository Interfaces

**IRepository<T>**
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

**IUnitOfWork**
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Store> Stores { get; }
    IRepository<Product> Products { get; }
    IRepository<Order> Orders { get; }
    IRepository<OrderItem> OrderItems { get; }
    IRepository<Payment> Payments { get; }
    IRepository<Review> Reviews { get; }
    IRepository<CartItem> CartItems { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

## Data Models

### Database Schema

**Users Table** (ASP.NET Identity)
```sql
CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) PRIMARY KEY,
    UserName NVARCHAR(256) NOT NULL,
    NormalizedUserName NVARCHAR(256),
    Email NVARCHAR(256) NOT NULL,
    NormalizedEmail NVARCHAR(256),
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX),
    SecurityStamp NVARCHAR(MAX),
    PhoneNumber NVARCHAR(50),
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
    FullName NVARCHAR(200),
    Address NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE INDEX IX_AspNetUsers_NormalizedEmail ON AspNetUsers(NormalizedEmail);
CREATE INDEX IX_AspNetUsers_NormalizedUserName ON AspNetUsers(NormalizedUserName);
```

**Roles Table** (ASP.NET Identity)
```sql
CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) PRIMARY KEY,
    Name NVARCHAR(256) NOT NULL,
    NormalizedName NVARCHAR(256),
    ConcurrencyStamp NVARCHAR(MAX)
);

CREATE INDEX IX_AspNetRoles_NormalizedName ON AspNetRoles(NormalizedName);
```

**UserRoles Table** (ASP.NET Identity)
```sql
CREATE TABLE AspNetUserRoles (
    UserId NVARCHAR(450) NOT NULL,
    RoleId NVARCHAR(450) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

CREATE INDEX IX_AspNetUserRoles_RoleId ON AspNetUserRoles(RoleId);
```

**Stores Table**
```sql
CREATE TABLE Stores (
    StoreId INT PRIMARY KEY IDENTITY(1,1),
    OwnerId NVARCHAR(450) NOT NULL,
    StoreName NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    Latitude DECIMAL(10, 8),
    Longitude DECIMAL(11, 8),
    LicenseNumber NVARCHAR(100) NOT NULL,
    DeliveryRadius DECIMAL(5, 2) NOT NULL,
    OpeningTime TIME NOT NULL,
    ClosingTime TIME NOT NULL,
    DeliveryCharge DECIMAL(10, 2) NOT NULL DEFAULT 0,
    StoreImageUrl NVARCHAR(500),
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    IsActive BIT NOT NULL DEFAULT 0,
    AverageRating DECIMAL(3, 2) DEFAULT 0,
    TotalReviews INT DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (OwnerId) REFERENCES AspNetUsers(Id)
);

CREATE INDEX IX_Stores_OwnerId ON Stores(OwnerId);
CREATE INDEX IX_Stores_Status ON Stores(Status);
CREATE INDEX IX_Stores_IsActive ON Stores(IsActive);
CREATE INDEX IX_Stores_Latitude_Longitude ON Stores(Latitude, Longitude);
```

**Categories Table**
```sql
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

CREATE INDEX IX_Categories_CategoryName ON Categories(CategoryName);
```

**Products Table**
```sql
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    StoreId INT NOT NULL,
    CategoryId INT NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    SKU NVARCHAR(100) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    DiscountPercentage DECIMAL(5, 2) DEFAULT 0,
    StockQuantity INT NOT NULL DEFAULT 0,
    ProductImageUrl NVARCHAR(500),
    IsActive BIT NOT NULL DEFAULT 1,
    AverageRating DECIMAL(3, 2) DEFAULT 0,
    TotalReviews INT DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (StoreId) REFERENCES Stores(StoreId) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId),
    CONSTRAINT UQ_Store_SKU UNIQUE (StoreId, SKU)
);

CREATE INDEX IX_Products_StoreId ON Products(StoreId);
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_ProductName ON Products(ProductName);
CREATE INDEX IX_Products_Price ON Products(Price);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
```

**Orders Table**
```sql
CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId NVARCHAR(450) NOT NULL,
    StoreId INT NOT NULL,
    OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
    DeliveryAddress NVARCHAR(500) NOT NULL,
    SubTotal DECIMAL(10, 2) NOT NULL,
    DeliveryCharge DECIMAL(10, 2) NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    RejectionReason NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (StoreId) REFERENCES Stores(StoreId)
);

CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId);
CREATE INDEX IX_Orders_StoreId ON Orders(StoreId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_OrderNumber ON Orders(OrderNumber);
CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt);
```

**OrderItems Table**
```sql
CREATE TABLE OrderItems (
    OrderItemId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductName NVARCHAR(200) NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    DiscountPercentage DECIMAL(5, 2) DEFAULT 0,
    LineTotal DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);
```

**Payments Table**
```sql
CREATE TABLE Payments (
    PaymentId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL UNIQUE,
    PaymentMethod NVARCHAR(50) NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    TransactionId NVARCHAR(200),
    PaymentGatewayResponse NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId) ON DELETE CASCADE
);

CREATE INDEX IX_Payments_OrderId ON Payments(OrderId);
CREATE INDEX IX_Payments_Status ON Payments(Status);
CREATE INDEX IX_Payments_TransactionId ON Payments(TransactionId);
```

**CartItems Table**
```sql
CREATE TABLE CartItems (
    CartItemId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId NVARCHAR(450) NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE,
    CONSTRAINT UQ_Customer_Product UNIQUE (CustomerId, ProductId)
);

CREATE INDEX IX_CartItems_CustomerId ON CartItems(CustomerId);
CREATE INDEX IX_CartItems_ProductId ON CartItems(ProductId);
```

**Reviews Table**
```sql
CREATE TABLE Reviews (
    ReviewId INT PRIMARY KEY IDENTITY(1,1),
    CustomerId NVARCHAR(450) NOT NULL,
    StoreId INT,
    ProductId INT,
    OrderId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(1000),
    IsApproved BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (StoreId) REFERENCES Stores(StoreId) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    CONSTRAINT CHK_Review_Target CHECK (
        (StoreId IS NOT NULL AND ProductId IS NULL) OR
        (StoreId IS NULL AND ProductId IS NOT NULL)
    )
);

CREATE INDEX IX_Reviews_CustomerId ON Reviews(CustomerId);
CREATE INDEX IX_Reviews_StoreId ON Reviews(StoreId);
CREATE INDEX IX_Reviews_ProductId ON Reviews(ProductId);
CREATE INDEX IX_Reviews_OrderId ON Reviews(OrderId);
```

**Notifications Table**
```sql
CREATE TABLE Notifications (
    NotificationId INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(450) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    RetryCount INT DEFAULT 0,
    SentAt DATETIME2,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

CREATE INDEX IX_Notifications_UserId ON Reviews(UserId);
CREATE INDEX IX_Notifications_Status ON Notifications(Status);
CREATE INDEX IX_Notifications_CreatedAt ON Notifications(CreatedAt);
```

### Entity Models

**Store Entity**
```csharp
public class Store
{
    public int StoreId { get; set; }
    public string OwnerId { get; set; }
    public string StoreName { get; set; }
    public string Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string LicenseNumber { get; set; }
    public decimal DeliveryRadius { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
    public decimal DeliveryCharge { get; set; }
    public string StoreImageUrl { get; set; }
    public StoreStatus Status { get; set; }
    public bool IsActive { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ApplicationUser Owner { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
```

**Product Entity**
```csharp
public class Product
{
    public int ProductId { get; set; }
    public int StoreId { get; set; }
    public int CategoryId { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public string SKU { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int StockQuantity { get; set; }
    public string ProductImageUrl { get; set; }
    public bool IsActive { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Store Store { get; set; }
    public Category Category { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<CartItem> CartItems { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
```

**Order Entity**
```csharp
public class Order
{
    public int OrderId { get; set; }
    public string CustomerId { get; set; }
    public int StoreId { get; set; }
    public string OrderNumber { get; set; }
    public string DeliveryAddress { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DeliveryCharge { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ApplicationUser Customer { get; set; }
    public Store Store { get; set; }
    public Payment Payment { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
```

### Enumerations

```csharp
public enum StoreStatus
{
    Pending,
    Approved,
    Rejected
}

public enum OrderStatus
{
    Pending,
    Accepted,
    Preparing,
    OutForDelivery,
    Delivered,
    Cancelled,
    Refunded
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Failed,
    Refunded,
    Completed
}

public enum PaymentMethod
{
    Online,
    CashOnDelivery
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*


### Property Reflection

After analyzing all acceptance criteria, I've identified several areas where properties can be consolidated:

**Consolidation Opportunities:**
- Properties 1.1, 1.2, 1.3 (user registration flow) can be combined into a registration round-trip property
- Properties 7.2 and 7.5 (cart subtotal calculation) are redundant - 7.5 subsumes 7.2
- Properties 11.4 and 11.5 (rating calculation) follow the same pattern and can be generalized
- Properties 14.1, 14.2, 14.3, 14.4 (notification triggering) follow the same pattern
- Properties 8.5, 8.6, 8.7 (order creation side effects) can be combined into order creation invariants
- Properties 3.3 and 3.4 (store approval/rejection) follow the same state transition pattern

**Unique Properties to Retain:**
- Authentication and authorization properties (distinct security concerns)
- Geo-location filtering (unique algorithm)
- Search and filter composition (complex query logic)
- Order lifecycle state machine (critical business logic)
- Payment processing (financial integrity)
- Stock management (inventory consistency)

### Core Correctness Properties

**Property 1: User Registration Creates Valid Account**
*For any* valid user registration data (name, email, phone, address, password), creating a user account should result in a new user record with hashed password, unverified email status, and Customer role assigned by default.
**Validates: Requirements 1.1, 1.6, 2.2**

**Property 2: Email Verification Round Trip**
*For any* user account, generating a verification token, then verifying with that token should mark the email as verified and allow login.
**Validates: Requirements 1.2, 1.3**

**Property 3: Unverified Users Cannot Login**
*For any* user account with unverified email, attempting to login should be rejected regardless of correct credentials.
**Validates: Requirements 1.4**

**Property 4: Duplicate Email Registration Rejected**
*For any* existing user email, attempting to register a new account with the same email should be rejected.
**Validates: Requirements 1.7**

**Property 5: Role-Based Authorization**
*For any* protected resource and user, access should be granted if and only if the user has the required role.
**Validates: Requirements 2.4, 2.5**

**Property 6: Store Registration Starts Pending**
*For any* store registration submitted by a StoreOwner, the created store record should have status "Pending" and IsActive set to false.
**Validates: Requirements 3.1, 3.2**

**Property 7: Store Approval State Transition**
*For any* pending store, admin approval should change status to "Approved" and set IsActive to true, while rejection should change status to "Rejected" and trigger notification.
**Validates: Requirements 3.3, 3.4**

**Property 8: Disabled Stores Hidden From Search**
*For any* customer search query, the results should never include stores where IsActive is false.
**Validates: Requirements 3.8**

**Property 9: Product SKU Uniqueness Within Store**
*For any* store, no two active products should have the same SKU code.
**Validates: Requirements 4.2**

**Property 10: Soft Delete Hides Products**
*For any* product marked as deleted (IsActive = false), it should not appear in any customer-facing product listings or search results.
**Validates: Requirements 4.4**

**Property 11: Zero Stock Prevents Orders**
*For any* product with StockQuantity = 0, attempting to add it to cart or checkout should be rejected.
**Validates: Requirements 4.6, 4.7**

**Property 12: Geo-Location Filtering**
*For any* customer address and store, the store should appear in customer's search results if and only if the calculated distance is less than or equal to the store's DeliveryRadius.
**Validates: Requirements 5.1, 5.2, 5.3**

**Property 13: Store Distance Sorting**
*For any* list of stores returned to a customer, the stores should be sorted by distance from customer's address in ascending order.
**Validates: Requirements 5.4**

**Property 14: Search Filter Composition**
*For any* product search with multiple filters (category, price range, store, rating), the results should match ALL filter criteria (AND logic, not OR).
**Validates: Requirements 6.6**

**Property 15: Cart Subtotal Calculation**
*For any* customer cart, the subtotal should equal the sum of (product price × (1 - discount percentage) × quantity) for all cart items.
**Validates: Requirements 7.2, 7.5**

**Property 16: Cart Quantity Stock Validation**
*For any* cart item, the quantity should never exceed the product's available StockQuantity.
**Validates: Requirements 7.4**

**Property 17: Cart Persistence Across Sessions**
*For any* logged-in customer, adding items to cart then logging out and back in should preserve all cart items.
**Validates: Requirements 7.6**

**Property 18: Order Creation Invariants**
*For any* successful order creation, the following should all be true: (1) order status is "Pending", (2) OrderItems match cart items exactly, (3) product stock quantities are reduced by ordered amounts, (4) customer's cart is cleared, (5) notifications are sent to customer and store owner.
**Validates: Requirements 8.4, 8.5, 8.6, 8.7, 8.8**

**Property 19: Checkout Stock Validation**
*For any* cart at checkout, if any cart item quantity exceeds the product's current StockQuantity, checkout should be rejected.
**Validates: Requirements 8.1**

**Property 20: Payment Status Transitions**
*For any* order with online payment, successful payment should set status to "Paid", failed payment should set status to "Failed", and COD should set status to "Pending".
**Validates: Requirements 9.3, 9.4, 9.5**

**Property 21: Payment Record Completeness**
*For any* order, the payment record should contain non-null values for amount, method, status, and (for online payments) transactionId.
**Validates: Requirements 9.6**

**Property 22: Order Lifecycle State Machine**
*For any* order, status transitions should follow valid sequences: Pending → Accepted → Preparing → OutForDelivery → Delivered, or Pending → Cancelled, with invalid transitions rejected.
**Validates: Requirements 10.2, 10.4, 10.6**

**Property 23: Order Rejection Triggers Refund**
*For any* order with payment status "Paid", rejecting the order should change order status to "Cancelled" and initiate a refund (payment status to "Refunded").
**Validates: Requirements 10.3**

**Property 24: COD Payment Completion**
*For any* order with payment method "CashOnDelivery", marking the order as "Delivered" should update payment status to "Completed".
**Validates: Requirements 10.7**

**Property 25: Review Eligibility**
*For any* order, customers should be able to create reviews if and only if the order status is "Delivered".
**Validates: Requirements 11.1, 11.3**

**Property 26: Average Rating Calculation**
*For any* store or product with reviews, the average rating should equal the sum of all review ratings divided by the count of reviews.
**Validates: Requirements 11.4, 11.5, 11.7**

**Property 27: Input Sanitization**
*For any* user input submitted to the system, SQL injection patterns should be escaped or rejected before database queries are executed.
**Validates: Requirements 15.2**

**Property 28: Account Lockout After Failed Attempts**
*For any* user account, 5 failed login attempts within 15 minutes should result in the account being locked (LockoutEnabled = true, LockoutEnd set to future time).
**Validates: Requirements 15.7**

**Property 29: Notification Retry Logic**
*For any* failed notification, the system should retry sending up to 3 times before marking it as permanently failed.
**Validates: Requirements 14.6**

**Property 30: Pagination Limit Enforcement**
*For any* list view query, the returned results should contain at most 50 items per page.
**Validates: Requirements 16.6**

## Error Handling

### Exception Handling Strategy

**Global Exception Handler**
- Implement a global exception filter to catch unhandled exceptions
- Log all exceptions with stack trace, user context, and request details
- Return appropriate HTTP status codes (400, 401, 403, 404, 500)
- Never expose sensitive error details to clients in production

**Validation Errors**
- Use Data Annotations for model validation
- Return 400 Bad Request with validation error details
- Implement custom validators for complex business rules
- Validate at both client-side (JavaScript) and server-side

**Business Logic Errors**
- Throw custom exceptions (e.g., InsufficientStockException, InvalidStateTransitionException)
- Map business exceptions to appropriate HTTP status codes
- Include user-friendly error messages
- Log business rule violations for analysis

**Database Errors**
- Catch DbUpdateException for constraint violations
- Handle concurrency conflicts with optimistic locking
- Retry transient failures (connection timeouts)
- Roll back transactions on any error

**External Service Errors**
- Implement circuit breaker pattern for payment gateway
- Handle timeout exceptions gracefully
- Provide fallback behavior when external services are unavailable
- Queue notifications for retry if email service fails

### Specific Error Scenarios

**Authentication Errors**
- Invalid credentials: Return 401 Unauthorized
- Unverified email: Return 403 Forbidden with specific message
- Account locked: Return 403 Forbidden with lockout duration
- Expired session: Return 401 and redirect to login

**Authorization Errors**
- Insufficient permissions: Return 403 Forbidden
- Resource not found: Return 404 Not Found
- Cross-tenant access attempt: Return 403 Forbidden

**Business Rule Violations**
- Out of stock: Return 400 with specific product details
- Invalid state transition: Return 400 with current and attempted states
- Duplicate SKU: Return 409 Conflict
- Store not approved: Return 403 Forbidden

**Data Validation Errors**
- Missing required fields: Return 400 with field names
- Invalid format: Return 400 with format requirements
- Value out of range: Return 400 with valid range
- Invalid file type/size: Return 400 with constraints

## Testing Strategy

### Dual Testing Approach

The system requires both unit testing and property-based testing for comprehensive coverage:

**Unit Tests** focus on:
- Specific examples demonstrating correct behavior
- Edge cases (empty carts, zero prices, boundary values)
- Error conditions (null inputs, invalid states)
- Integration points between components
- Mock external dependencies (payment gateway, email service)

**Property-Based Tests** focus on:
- Universal properties that hold for all inputs
- Comprehensive input coverage through randomization
- Invariants that must always be maintained
- State machine transitions
- Calculation correctness across all values

### Property-Based Testing Configuration

**Framework**: Use **FsCheck** for C# property-based testing
- Minimum 100 iterations per property test
- Configure custom generators for domain objects
- Use Arbitrary<T> for complex type generation
- Seed random generator for reproducible failures

**Test Organization**:
- Create separate test project: LocalStorePlatform.PropertyTests
- Group tests by feature area (Authentication, Orders, Cart, etc.)
- Tag each test with feature name and property number
- Reference design document properties in test comments

**Example Property Test Structure**:
```csharp
[Property]
public Property CartSubtotalCalculation()
{
    return Prop.ForAll<List<CartItem>>(cartItems =>
    {
        // Feature: local-store-platform, Property 15: Cart Subtotal Calculation
        var cart = new Cart { Items = cartItems };
        var expectedSubtotal = cartItems.Sum(item => 
            item.Product.Price * (1 - item.Product.DiscountPercentage / 100) * item.Quantity);
        var actualSubtotal = _cartService.CalculateSubtotal(cart);
        return actualSubtotal == expectedSubtotal;
    });
}
```

### Unit Testing Strategy

**Test Coverage Goals**:
- Minimum 80% code coverage
- 100% coverage for critical paths (payment, order creation, stock management)
- All public methods in services should have tests
- All controller actions should have tests

**Testing Frameworks**:
- **xUnit**: Primary testing framework
- **Moq**: Mocking framework for dependencies
- **FluentAssertions**: Readable assertions
- **AutoFixture**: Test data generation

**Test Categories**:
1. **Service Layer Tests**: Test business logic in isolation
2. **Repository Tests**: Test data access with in-memory database
3. **Controller Tests**: Test HTTP request/response handling
4. **Integration Tests**: Test complete workflows end-to-end
5. **Validation Tests**: Test model validation rules

**Example Unit Test Structure**:
```csharp
public class OrderServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly OrderService _orderService;

    [Fact]
    public async Task CreateOrder_WithValidCart_CreatesOrderAndClearsCart()
    {
        // Arrange
        var customerId = "customer123";
        var cartItems = CreateTestCartItems();
        
        // Act
        var order = await _orderService.CreateOrderAsync(customerId, checkoutDto);
        
        // Assert
        order.Status.Should().Be(OrderStatus.Pending);
        order.OrderItems.Should().HaveCount(cartItems.Count);
        // Verify cart was cleared
        _unitOfWorkMock.Verify(u => u.CartItems.DeleteAsync(It.IsAny<CartItem>()), 
            Times.Exactly(cartItems.Count));
    }
}
```

### Integration Testing

**Test Database**:
- Use SQL Server LocalDB or in-memory SQLite for tests
- Reset database state between tests
- Seed test data using fixtures

**End-to-End Scenarios**:
1. Complete user registration and login flow
2. Store registration, approval, and product creation
3. Product search, add to cart, and checkout
4. Order lifecycle from creation to delivery
5. Payment processing (with mocked gateway)
6. Review submission and rating calculation

### Performance Testing

**Load Testing**:
- Use Apache JMeter or k6 for load testing
- Test with 1000+ concurrent users
- Measure response times under load
- Identify bottlenecks and slow queries

**Benchmarking**:
- Use BenchmarkDotNet for micro-benchmarks
- Test critical algorithms (geo-location calculation, search filtering)
- Optimize based on benchmark results

### Security Testing

**Automated Security Scans**:
- Use OWASP ZAP for vulnerability scanning
- Test for SQL injection, XSS, CSRF
- Verify authentication and authorization
- Test rate limiting and account lockout

**Manual Security Review**:
- Code review for security best practices
- Review all user input handling
- Verify encryption of sensitive data
- Test session management

## Deployment Considerations

### Database Migration Strategy

- Use Entity Framework Core Migrations
- Version control all migration files
- Test migrations on staging before production
- Implement rollback scripts for each migration
- Backup database before applying migrations

### Configuration Management

- Use appsettings.json for environment-specific settings
- Store secrets in Azure Key Vault or environment variables
- Never commit sensitive data to source control
- Use different connection strings per environment

### Monitoring and Logging

**Logging Strategy**:
- Use Serilog with structured logging
- Log levels: Debug, Information, Warning, Error, Fatal
- Log to multiple sinks: File, SQL Server, Application Insights
- Include correlation IDs for request tracing

**Monitoring Metrics**:
- Application performance (response times, throughput)
- Error rates and exception counts
- Database query performance
- External service availability
- User activity metrics

### Scalability Considerations

**Caching Strategy**:
- Cache product listings (5-minute expiration)
- Cache store information (10-minute expiration)
- Cache category lists (1-hour expiration)
- Invalidate cache on data updates

**Database Optimization**:
- Implement all indexes defined in schema
- Use query optimization hints where needed
- Partition large tables (Orders, OrderItems)
- Archive old data periodically

**Horizontal Scaling**:
- Design for stateless web servers
- Use distributed cache (Redis) for multi-server deployments
- Implement database read replicas for read-heavy operations
- Use load balancer for traffic distribution

## Future Enhancements

### Phase 2 Features

1. **Real-time Delivery Tracking**
   - GPS integration for delivery partners
   - Live order status updates via SignalR
   - Estimated delivery time calculation

2. **Advanced Search**
   - Elasticsearch integration for full-text search
   - Autocomplete suggestions
   - Search history and recommendations

3. **Mobile Application**
   - Native iOS and Android apps
   - Push notifications
   - Offline mode support

4. **Analytics Dashboard**
   - Advanced reporting with charts
   - Predictive analytics for demand forecasting
   - Customer behavior analysis

5. **Loyalty Program**
   - Points-based rewards system
   - Referral bonuses
   - Subscription-based delivery passes

### Technical Debt and Improvements

- Migrate to microservices architecture for better scalability
- Implement CQRS pattern for read/write separation
- Add GraphQL API for flexible client queries
- Implement event sourcing for audit trail
- Add comprehensive API documentation with Swagger
