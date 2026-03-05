# Implementation Plan: Local Store Delivery Platform

## Overview

This implementation plan breaks down the Local Store Delivery Platform into incremental, testable steps. The approach follows a layered architecture pattern, building from the data layer up through business logic to the presentation layer. Each major feature area is implemented with its core functionality first, followed by property-based tests to validate correctness properties from the design document.

The implementation uses ASP.NET Core MVC 6.0+ with Entity Framework Core, ASP.NET Identity for authentication, and follows repository and service patterns for clean separation of concerns.

## Tasks

- [x] 1. Project setup and infrastructure
  - Create ASP.NET Core MVC project with folder structure (Controllers, Models, Services, Repositories, Views)
  - Configure Entity Framework Core with SQL Server connection
  - Set up dependency injection container
  - Configure Serilog for logging
  - Add required NuGet packages (EF Core, Identity, MailKit, FsCheck for property testing)
  - _Requirements: 16.3, 16.4_

- [x] 2. Database schema and migrations
  - [x] 2.1 Create Entity Framework entity models
    - Create ApplicationUser extending IdentityUser with additional properties
    - Create Store, Product, Category, Order, OrderItem, Payment, CartItem, Review, Notification entities
    - Define enums: StoreStatus, OrderStatus, PaymentStatus, PaymentMethod, NotificationStatus
    - Configure entity relationships and navigation properties
    - _Requirements: 1.1, 3.1, 4.1, 8.4, 9.6, 11.1_

  - [x] 2.2 Configure Entity Framework DbContext
    - Create ApplicationDbContext inheriting from IdentityDbContext
    - Configure entity mappings using Fluent API
    - Define indexes on frequently queried columns
    - Set up unique constraints (Store SKU, Order Number)
    - Configure cascade delete behaviors
    - _Requirements: 4.2, 16.3_

  - [x] 2.3 Create initial database migration
    - Generate EF Core migration for all entities
    - Review generated SQL for correctness
    - Apply migration to create database schema
    - _Requirements: All data model requirements_


- [x] 3. Repository pattern implementation
  - [x] 3.1 Create generic repository interface and implementation
    - Implement IRepository<T> with GetByIdAsync, GetAllAsync, FindAsync, AddAsync, UpdateAsync, DeleteAsync
    - Create GenericRepository<T> implementation using DbContext
    - _Requirements: All data access requirements_

  - [x] 3.2 Create Unit of Work pattern
    - Implement IUnitOfWork interface with repository properties
    - Create UnitOfWork class managing DbContext and transactions
    - Implement SaveChangesAsync, BeginTransactionAsync, CommitTransactionAsync, RollbackTransactionAsync
    - _Requirements: 8.6, 10.3_

  - [ ]* 3.3 Write unit tests for repository layer
    - Test CRUD operations using in-memory database
    - Test transaction rollback scenarios
    - _Requirements: 3.1, 4.1, 8.4_

- [x] 4. Authentication and authorization
  - [x] 4.1 Configure ASP.NET Core Identity
    - Configure Identity services in Program.cs
    - Set password requirements and lockout settings
    - Configure cookie authentication
    - Seed initial roles (Admin, StoreOwner, Customer)
    - _Requirements: 1.1, 1.6, 2.1, 15.7_

  - [x] 4.2 Implement authentication service
    - Create IAuthenticationService interface
    - Implement RegisterAsync with email verification token generation
    - Implement LoginAsync with email verification check
    - Implement LogoutAsync
    - Implement SendEmailVerificationAsync and VerifyEmailAsync
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.7_

  - [ ]* 4.3 Write property test for user registration
    - **Property 1: User Registration Creates Valid Account**
    - **Validates: Requirements 1.1, 1.6, 2.2**

  - [ ]* 4.4 Write property test for email verification
    - **Property 2: Email Verification Round Trip**
    - **Validates: Requirements 1.2, 1.3**

  - [ ]* 4.5 Write property test for unverified user login prevention
    - **Property 3: Unverified Users Cannot Login**
    - **Validates: Requirements 1.4**

  - [ ]* 4.6 Write property test for duplicate email rejection
    - **Property 4: Duplicate Email Registration Rejected**
    - **Validates: Requirements 1.7**

  - [x] 4.7 Create authentication controllers and views
    - Create AccountController with Register, Login, Logout, VerifyEmail actions
    - Create Razor views for registration and login forms
    - Implement client-side validation with jQuery
    - _Requirements: 1.1, 1.5_

  - [ ]* 4.8 Write property test for role-based authorization
    - **Property 5: Role-Based Authorization**
    - **Validates: Requirements 2.4, 2.5**

- [ ] 5. Checkpoint - Ensure authentication tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [x] 6. Store management
  - [x] 6.1 Implement store service
    - Create IStoreService interface
    - Implement CreateStoreAsync setting status to Pending
    - Implement UpdateStoreAsync
    - Implement ApproveStoreAsync and RejectStoreAsync (admin only)
    - Implement ToggleStoreStatusAsync
    - Implement GetStoreByIdAsync and GetNearbyStoresAsync
    - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.7, 5.1_

  - [ ] 6.2 Write property test for store registration
    - **Property 6: Store Registration Starts Pending**
    - **Validates: Requirements 3.1, 3.2**

  - [ ] 6.3 Write property test for store approval
    - **Property 7: Store Approval State Transition**
    - **Validates: Requirements 3.3, 3.4**

  - [ ]* 6.4 Write property test for disabled store filtering
    - **Property 8: Disabled Stores Hidden From Search**
    - **Validates: Requirements 3.8**

  - [x] 6.5 Create store controllers and views
    - Create StoreController with Create, Edit, Details, ToggleStatus actions (StoreOwner role)
    - Create AdminStoreController with ApproveStore, RejectStore actions (Admin role)
    - Create Razor views for store registration and management
    - Implement file upload for store images
    - _Requirements: 3.1, 3.3, 3.4, 3.5, 3.6, 3.7_

- [x] 7. Geo-location service
  - [x] 7.1 Implement geo-location service
    - Create IGeoLocationService interface
    - Implement CalculateDistance using Haversine formula
    - Implement GeocodeAddressAsync (optional: integrate Google Maps API or use simple parsing)
    - _Requirements: 5.1_

  - [ ]* 7.2 Write property test for geo-location filtering
    - **Property 12: Geo-Location Filtering**
    - **Validates: Requirements 5.1, 5.2, 5.3**

  - [ ]* 7.3 Write property test for store distance sorting
    - **Property 13: Store Distance Sorting**
    - **Validates: Requirements 5.4**

  - [ ]* 7.4 Write unit tests for Haversine formula
    - Test with known coordinate pairs and expected distances
    - Test edge cases (same location, antipodal points)
    - _Requirements: 5.1_

- [x] 8. Product catalog management
  - [x] 8.1 Implement category management
    - Create ICategoryService interface
    - Implement CRUD operations for categories
    - Create AdminCategoryController (Admin role)
    - Create Razor views for category management
    - _Requirements: 12.3_

  - [x] 8.2 Implement product service
    - Create IProductService interface
    - Implement CreateProductAsync with SKU uniqueness validation
    - Implement UpdateProductAsync with timestamp update
    - Implement DeleteProductAsync (soft delete)
    - Implement UpdateStockAsync
    - Implement SearchProductsAsync with filtering and geo-location
    - Implement GetProductByIdAsync
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 6.1, 6.2, 6.3, 6.4, 6.5_

  - [ ]* 8.3 Write property test for SKU uniqueness
    - **Property 9: Product SKU Uniqueness Within Store**
    - **Validates: Requirements 4.2**

  - [ ]* 8.4 Write property test for soft delete
    - **Property 10: Soft Delete Hides Products**
    - **Validates: Requirements 4.4**

  - [ ]* 8.5 Write property test for zero stock prevention
    - **Property 11: Zero Stock Prevents Orders**
    - **Validates: Requirements 4.6, 4.7**

  - [x] 8.6 Create product controllers and views
    - Create ProductController with Create, Edit, Delete, Details actions (StoreOwner role)
    - Create Razor views for product management
    - Implement file upload for product images
    - Add stock quantity management UI
    - _Requirements: 4.1, 4.3, 4.4, 4.5_

- [ ] 9. Product search and browsing
  - [x] 9.1 Implement product search with filters
    - Create ProductSearchDto with search term, category, price range, store, rating filters
    - Implement search logic in ProductService combining all filters
    - Implement geo-location filtering in search
    - _Requirements: 5.3, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_

  - [ ]* 9.2 Write property test for filter composition
    - **Property 14: Search Filter Composition**
    - **Validates: Requirements 6.6**

  - [x] 9.3 Create customer product browsing views
    - Create HomeController with Index action showing nearby stores
    - Create ProductBrowseController with Search action
    - Create Razor views for product listing and search
    - Implement AJAX-based filtering
    - Display product cards with name, price, discount, store, rating
    - _Requirements: 5.2, 5.4, 6.1, 6.7_

- [ ] 10. Checkpoint - Ensure store and product tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 11. Shopping cart
  - [x] 11.1 Implement cart service
    - Create ICartService interface
    - Implement AddToCartAsync with stock validation
    - Implement UpdateCartItemAsync with subtotal recalculation
    - Implement RemoveFromCartAsync
    - Implement GetCartAsync
    - Implement GetCartSubtotalAsync
    - Implement ClearCartAsync
    - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7_

  - [ ]* 11.2 Write property test for cart subtotal calculation
    - **Property 15: Cart Subtotal Calculation**
    - **Validates: Requirements 7.2, 7.5**

  - [ ]* 11.3 Write property test for stock validation
    - **Property 16: Cart Quantity Stock Validation**
    - **Validates: Requirements 7.4**

  - [ ]* 11.4 Write property test for cart persistence
    - **Property 17: Cart Persistence Across Sessions**
    - **Validates: Requirements 7.6**

  - [x] 11.5 Create cart controllers and views
    - Create CartController with AddToCart, UpdateQuantity, RemoveItem, ViewCart actions
    - Create Razor view for cart display
    - Implement AJAX for cart operations
    - Display cart items with product details and line totals
    - _Requirements: 7.1, 7.2, 7.3, 7.7_

- [x] 12. Checkout and order placement
  - [x] 12.1 Implement order service
    - Create IOrderService interface
    - Implement CreateOrderAsync with stock validation, order creation, stock reduction, cart clearing
    - Generate unique order numbers
    - Implement UpdateOrderStatusAsync with state machine validation
    - Implement AcceptOrderAsync and RejectOrderAsync (StoreOwner role)
    - Implement GetOrdersByCustomerAsync, GetOrdersByStoreAsync, GetOrderByIdAsync
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 8.7, 10.2, 10.3, 10.4, 10.6_

  - [ ]* 12.2 Write property test for order creation invariants
    - **Property 18: Order Creation Invariants**
    - **Validates: Requirements 8.4, 8.5, 8.6, 8.7, 8.8**

  - [ ]* 12.3 Write property test for checkout stock validation
    - **Property 19: Checkout Stock Validation**
    - **Validates: Requirements 8.1**

  - [ ]* 12.4 Write property test for order lifecycle state machine
    - **Property 22: Order Lifecycle State Machine**
    - **Validates: Requirements 10.2, 10.4, 10.6**

  - [x] 12.5 Create checkout controllers and views
    - Create CheckoutController with Checkout, ConfirmOrder actions
    - Create Razor views for checkout flow
    - Implement address selection
    - Implement payment method selection
    - Display order summary before confirmation
    - _Requirements: 8.2, 8.3, 8.4_

- [ ] 13. Payment processing
  - [ ] 13.1 Implement payment service
    - Create IPaymentService interface
    - Implement ProcessOnlinePaymentAsync integrating with payment gateway (Stripe/Razorpay)
    - Implement UpdatePaymentStatusAsync
    - Implement InitiateRefundAsync
    - Implement GetPaymentByOrderIdAsync
    - Handle payment webhooks for status updates
    - _Requirements: 9.2, 9.3, 9.4, 9.5, 9.6, 9.7, 10.3_

  - [ ]* 13.2 Write property test for payment status transitions
    - **Property 20: Payment Status Transitions**
    - **Validates: Requirements 9.3, 9.4, 9.5**

  - [ ]* 13.3 Write property test for payment record completeness
    - **Property 21: Payment Record Completeness**
    - **Validates: Requirements 9.6**

  - [ ]* 13.4 Write property test for order rejection refund
    - **Property 23: Order Rejection Triggers Refund**
    - **Validates: Requirements 10.3**

  - [ ]* 13.5 Write property test for COD payment completion
    - **Property 24: COD Payment Completion**
    - **Validates: Requirements 10.7**

  - [ ] 13.6 Create payment controllers
    - Create PaymentController with ProcessPayment, PaymentCallback actions
    - Handle payment gateway redirects
    - Display payment success/failure messages
    - _Requirements: 9.2, 9.3, 9.4_

- [x] 14. Order management
  - [x] 14.1 Create order management controllers and views
    - Create OrderController with MyOrders, OrderDetails actions (Customer role)
    - Create StoreOrderController with StoreOrders, AcceptOrder, RejectOrder, UpdateStatus actions (StoreOwner role)
    - Create Razor views for order listing and details
    - Display order status timeline
    - Implement order filtering by status and date
    - _Requirements: 10.2, 10.3, 10.4, 13.6_

  - [ ]* 14.2 Write unit tests for order status updates
    - Test valid status transitions
    - Test invalid status transitions are rejected
    - Test notification triggering on status change
    - _Requirements: 10.4, 10.5, 10.6_

- [ ] 15. Checkpoint - Ensure order and payment tests pass
  - Ensure all tests pass, ask the user if questions arise.

- [x] 16. Rating and review system
  - [x] 16.1 Implement review service
    - Create IReviewService interface
    - Implement CreateStoreReviewAsync with delivery status validation
    - Implement CreateProductReviewAsync with delivery status validation
    - Implement DeleteReviewAsync (Admin role)
    - Implement CalculateStoreRatingAsync and CalculateProductRatingAsync
    - Implement GetStoreReviewsAsync
    - Update store/product average rating after review creation
    - _Requirements: 11.1, 11.2, 11.3, 11.4, 11.5, 11.6, 11.7_

  - [ ]* 16.2 Write property test for review eligibility
    - **Property 25: Review Eligibility**
    - **Validates: Requirements 11.1, 11.3**

  - [ ]* 16.3 Write property test for average rating calculation
    - **Property 26: Average Rating Calculation**
    - **Validates: Requirements 11.4, 11.5, 11.7**

  - [x] 16.4 Create review controllers and views
    - Create ReviewController with CreateReview, ViewReviews actions
    - Create AdminReviewController with DeleteReview action (Admin role)
    - Create Razor views for review submission and display
    - Display star ratings with visual representation
    - _Requirements: 11.1, 11.2, 11.6_

- [x] 17. Notification system
  - [x] 17.1 Implement notification service
    - Create INotificationService interface
    - Implement SendEmailAsync using MailKit/SMTP
    - Implement SendOrderConfirmationAsync
    - Implement SendOrderStatusUpdateAsync
    - Implement SendStoreApprovalAsync
    - Implement SendLowStockAlertAsync
    - Log all notification attempts to database
    - Implement retry logic for failed notifications
    - _Requirements: 8.8, 10.5, 14.1, 14.2, 14.3, 14.4, 14.5, 14.6_

  - [ ]* 17.2 Write property test for notification retry logic
    - **Property 29: Notification Retry Logic**
    - **Validates: Requirements 14.6**

  - [ ]* 17.3 Write unit tests for notification triggering
    - Test order confirmation notification
    - Test status update notification
    - Test store approval notification
    - Test low stock alert
    - _Requirements: 14.1, 14.2, 14.3, 14.4_

  - [x] 17.4 Implement background job for notification processing
    - Use Hangfire or similar for background job processing
    - Create job to process pending notifications
    - Create job to retry failed notifications
    - Schedule low stock alert job to run daily
    - _Requirements: 14.4, 14.6_

- [x] 18. Admin dashboard
  - [x] 18.1 Implement admin dashboard service
    - Create IAdminDashboardService interface
    - Implement GetTotalSalesAsync
    - Implement GetActiveStoresCountAsync
    - Implement GetTotalOrdersCountAsync
    - Implement GetTopSellingProductsAsync
    - Implement GetMonthlyRevenueTrendsAsync
    - _Requirements: 12.1, 12.5, 12.6_

  - [x] 18.2 Create admin dashboard controllers and views
    - Create AdminDashboardController with Dashboard action (Admin role)
    - Create Razor view displaying key metrics
    - Implement charts for revenue trends using Chart.js
    - Display pending store approvals
    - Display recent orders
    - _Requirements: 12.1, 12.2, 12.4, 12.5, 12.6_

  - [x] 18.3 Implement admin user management
    - Create AdminUserController with ListUsers, DisableUser actions (Admin role)
    - Create Razor views for user management
    - Implement user search and filtering
    - _Requirements: 12.7_

- [x] 19. Store owner dashboard
  - [x] 19.1 Implement store dashboard service
    - Create IStoreDashboardService interface
    - Implement GetDailySalesAsync
    - Implement GetPendingOrdersCountAsync
    - Implement GetLowStockProductsAsync
    - Implement GetMonthlyStatsAsync
    - _Requirements: 13.1, 13.2, 13.3, 13.4_

  - [x] 19.2 Create store dashboard controllers and views
    - Create StoreDashboardController with Dashboard action (StoreOwner role)
    - Create Razor view displaying store metrics
    - Display pending orders requiring action
    - Display low stock alerts
    - Display average store rating
    - _Requirements: 13.1, 13.2, 13.3, 13.4, 13.5, 13.6_

- [x] 20. Security implementation
  - [x] 20.1 Implement security measures
    - Configure HTTPS redirection in Program.cs
    - Add CSRF token validation to all forms
    - Implement input sanitization middleware
    - Configure rate limiting middleware
    - Implement account lockout after failed login attempts
    - Add security headers (HSTS, X-Frame-Options, etc.)
    - _Requirements: 15.1, 15.2, 15.3, 15.6, 15.7_

  - [ ]* 20.2 Write property test for input sanitization
    - **Property 27: Input Sanitization**
    - **Validates: Requirements 15.2**

  - [ ]* 20.3 Write property test for account lockout
    - **Property 28: Account Lockout After Failed Attempts**
    - **Validates: Requirements 15.7**

  - [x] 20.4 Implement audit logging
    - Log all authentication attempts with IP and timestamp
    - Log all authorization failures
    - Log all data modifications with user context
    - _Requirements: 15.5_

- [x] 21. Performance optimization
  - [x] 21.1 Implement caching
    - Configure IMemoryCache in Program.cs
    - Cache product listings with 5-minute expiration
    - Cache store information with 10-minute expiration
    - Cache category lists with 1-hour expiration
    - Implement cache invalidation on data updates
    - _Requirements: 16.4_

  - [x] 21.2 Implement pagination
    - Create PaginatedList<T> helper class
    - Add pagination to all list views (products, orders, stores)
    - Set maximum page size to 50 items
    - _Requirements: 16.6_

  - [ ]* 21.3 Write property test for pagination limit
    - **Property 30: Pagination Limit Enforcement**
    - **Validates: Requirements 16.6**

  - [x] 21.4 Optimize database queries
    - Add .AsNoTracking() for read-only queries
    - Use eager loading (.Include()) to prevent N+1 queries
    - Implement query result caching where appropriate
    - Log slow queries exceeding 1 second
    - _Requirements: 16.3, 16.7_

- [x] 22. Final integration and testing
  - [x] 22.1 Create database seed data
    - Seed admin user account
    - Seed sample categories
    - Seed sample stores and products for testing
    - Create data seeding script for development environment
    - _Requirements: 2.1_

  - [ ]* 22.2 Write integration tests for complete workflows
    - Test complete user registration to order placement flow
    - Test store registration to product sale flow
    - Test order lifecycle from creation to delivery
    - Test payment processing end-to-end
    - _Requirements: All requirements_

  - [ ] 22.3 Perform security testing
    - Test SQL injection prevention
    - Test XSS prevention
    - Test CSRF protection
    - Test authentication and authorization
    - Test rate limiting
    - _Requirements: 15.2, 15.3, 15.6_

  - [x] 22.4 Create deployment documentation
    - Document database setup and migration steps
    - Document configuration settings
    - Document environment variables and secrets
    - Create deployment checklist
    - _Requirements: All requirements_

- [ ] 23. Final checkpoint - Ensure all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation at major milestones
- Property tests validate universal correctness properties using FsCheck
- Unit tests validate specific examples and edge cases using xUnit
- Integration tests validate complete workflows end-to-end
- The implementation follows a bottom-up approach: data layer → business logic → presentation
- All property tests should run with minimum 100 iterations
- Security and performance considerations are integrated throughout
