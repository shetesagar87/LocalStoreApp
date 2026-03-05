using CleanMvcApp.Models.Entities;
using CleanMvcApp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanMvcApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed roles
            string[] roleNames = { "Admin", "StoreOwner", "Customer" };
            
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Admin user
            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "admin@localstore.com",
                Email = "admin@localstore.com",
                FullName = "System Administrator",
                Address = "123 Admin Street, City Center",
                PhoneNumber = "+1234567890",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Admin@123", "Admin");

            // Seed StoreOwner users
            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "owner1@localstore.com",
                Email = "owner1@localstore.com",
                FullName = "John Smith",
                Address = "456 Market Street, Downtown",
                PhoneNumber = "+1234567891",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Owner@123", "StoreOwner");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "owner2@localstore.com",
                Email = "owner2@localstore.com",
                FullName = "Sarah Johnson",
                Address = "789 Commerce Ave, Business District",
                PhoneNumber = "+1234567892",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Owner@123", "StoreOwner");

            // Seed Customer users
            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "customer1@localstore.com",
                Email = "customer1@localstore.com",
                FullName = "Michael Brown",
                Address = "321 Residential Lane, Suburb Area",
                PhoneNumber = "+1234567893",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Customer@123", "Customer");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "customer2@localstore.com",
                Email = "customer2@localstore.com",
                FullName = "Emily Davis",
                Address = "654 Park View, Green Valley",
                PhoneNumber = "+1234567894",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Customer@123", "Customer");

            await SeedUserAsync(userManager, new ApplicationUser
            {
                UserName = "customer3@localstore.com",
                Email = "customer3@localstore.com",
                FullName = "David Wilson",
                Address = "987 Lake Side Drive, Waterfront",
                PhoneNumber = "+1234567895",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, "Customer@123", "Customer");
        }

        private static async Task SeedUserAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string password,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(user.Email);

            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        public static async Task SeedDemoDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Check if data already exists
            if (await context.Categories.AnyAsync() || await context.Stores.AnyAsync())
            {
                return; // Data already seeded
            }

            // Get users
            var owner1 = await userManager.FindByEmailAsync("owner1@localstore.com");
            var owner2 = await userManager.FindByEmailAsync("owner2@localstore.com");
            var customer1 = await userManager.FindByEmailAsync("customer1@localstore.com");
            var customer2 = await userManager.FindByEmailAsync("customer2@localstore.com");
            var customer3 = await userManager.FindByEmailAsync("customer3@localstore.com");

            if (owner1 == null || owner2 == null || customer1 == null)
            {
                return; // Users not found
            }

            // Seed Categories
            var categories = new List<Category>
            {
                new Category { CategoryName = "Electronics", Description = "Electronic devices and accessories", CreatedAt = DateTime.UtcNow },
                new Category { CategoryName = "Groceries", Description = "Fresh food and daily essentials", CreatedAt = DateTime.UtcNow },
                new Category { CategoryName = "Clothing", Description = "Fashion and apparel", CreatedAt = DateTime.UtcNow },
                new Category { CategoryName = "Books", Description = "Books and stationery", CreatedAt = DateTime.UtcNow },
                new Category { CategoryName = "Home & Kitchen", Description = "Home appliances and kitchenware", CreatedAt = DateTime.UtcNow },
                new Category { CategoryName = "Sports", Description = "Sports equipment and fitness", CreatedAt = DateTime.UtcNow },
                new Category { CategoryName = "Toys", Description = "Toys and games for kids", CreatedAt = DateTime.UtcNow },
                new Category { CategoryName = "Beauty", Description = "Beauty and personal care", CreatedAt = DateTime.UtcNow }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // Seed Stores
            var store1 = new Store
            {
                OwnerId = owner1.Id,
                StoreName = "Tech Haven Electronics",
                Address = "123 Tech Street, Silicon Valley, CA 94025",
                Latitude = 37.4419m,
                Longitude = -122.1430m,
                LicenseNumber = "LIC-2024-001",
                DeliveryRadius = 10.0m,
                OpeningTime = new TimeSpan(9, 0, 0),
                ClosingTime = new TimeSpan(21, 0, 0),
                DeliveryCharge = 5.99m,
                Status = StoreStatus.Approved,
                IsActive = true,
                AverageRating = 4.5m,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            };

            var store2 = new Store
            {
                OwnerId = owner1.Id,
                StoreName = "Fresh Mart Groceries",
                Address = "456 Market Avenue, Downtown, CA 94102",
                Latitude = 37.7749m,
                Longitude = -122.4194m,
                LicenseNumber = "LIC-2024-002",
                DeliveryRadius = 8.0m,
                OpeningTime = new TimeSpan(7, 0, 0),
                ClosingTime = new TimeSpan(22, 0, 0),
                DeliveryCharge = 3.99m,
                Status = StoreStatus.Approved,
                IsActive = true,
                AverageRating = 4.7m,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = DateTime.UtcNow
            };

            var store3 = new Store
            {
                OwnerId = owner2.Id,
                StoreName = "Fashion Hub",
                Address = "789 Style Boulevard, Fashion District, CA 90015",
                Latitude = 34.0407m,
                Longitude = -118.2468m,
                LicenseNumber = "LIC-2024-003",
                DeliveryRadius = 12.0m,
                OpeningTime = new TimeSpan(10, 0, 0),
                ClosingTime = new TimeSpan(20, 0, 0),
                DeliveryCharge = 4.99m,
                Status = StoreStatus.Approved,
                IsActive = true,
                AverageRating = 4.3m,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow
            };

            var store4 = new Store
            {
                OwnerId = owner2.Id,
                StoreName = "Book Nook",
                Address = "321 Library Lane, University Area, CA 94720",
                Latitude = 37.8715m,
                Longitude = -122.2730m,
                LicenseNumber = "LIC-2024-004",
                DeliveryRadius = 7.0m,
                OpeningTime = new TimeSpan(8, 0, 0),
                ClosingTime = new TimeSpan(19, 0, 0),
                DeliveryCharge = 2.99m,
                Status = StoreStatus.Approved,
                IsActive = true,
                AverageRating = 4.8m,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow
            };

            await context.Stores.AddRangeAsync(new[] { store1, store2, store3, store4 });
            await context.SaveChangesAsync();

            // Seed Products for Store 1 (Tech Haven)
            var electronicsCategory = categories.First(c => c.CategoryName == "Electronics");
            var products1 = new List<Product>
            {
                new Product
                {
                    StoreId = store1.StoreId,
                    CategoryId = electronicsCategory.CategoryId,
                    ProductName = "Wireless Bluetooth Headphones",
                    Description = "Premium noise-cancelling wireless headphones with 30-hour battery life",
                    SKU = "TH-WH-001",
                    Price = 79.99m,
                    DiscountPercentage = 15,
                    StockQuantity = 50,
                    AverageRating = 4.6m,
                    CreatedAt = DateTime.UtcNow.AddDays(-28),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store1.StoreId,
                    CategoryId = electronicsCategory.CategoryId,
                    ProductName = "Smart Watch Pro",
                    Description = "Fitness tracking smartwatch with heart rate monitor and GPS",
                    SKU = "TH-SW-002",
                    Price = 199.99m,
                    DiscountPercentage = 20,
                    StockQuantity = 30,
                    AverageRating = 4.7m,
                    CreatedAt = DateTime.UtcNow.AddDays(-27),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store1.StoreId,
                    CategoryId = electronicsCategory.CategoryId,
                    ProductName = "USB-C Fast Charger",
                    Description = "65W fast charging adapter with multiple ports",
                    SKU = "TH-CH-003",
                    Price = 29.99m,
                    DiscountPercentage = 10,
                    StockQuantity = 100,
                    AverageRating = 4.4m,
                    CreatedAt = DateTime.UtcNow.AddDays(-26),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store1.StoreId,
                    CategoryId = electronicsCategory.CategoryId,
                    ProductName = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with precision tracking",
                    SKU = "TH-MS-004",
                    Price = 24.99m,
                    DiscountPercentage = 0,
                    StockQuantity = 75,
                    AverageRating = 4.3m,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store1.StoreId,
                    CategoryId = electronicsCategory.CategoryId,
                    ProductName = "Portable Power Bank 20000mAh",
                    Description = "High-capacity power bank with fast charging support",
                    SKU = "TH-PB-005",
                    Price = 39.99m,
                    DiscountPercentage = 25,
                    StockQuantity = 60,
                    AverageRating = 4.5m,
                    CreatedAt = DateTime.UtcNow.AddDays(-24),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Seed Products for Store 2 (Fresh Mart)
            var groceriesCategory = categories.First(c => c.CategoryName == "Groceries");
            var products2 = new List<Product>
            {
                new Product
                {
                    StoreId = store2.StoreId,
                    CategoryId = groceriesCategory.CategoryId,
                    ProductName = "Organic Fresh Milk (1 Gallon)",
                    Description = "Farm-fresh organic whole milk",
                    SKU = "FM-MLK-001",
                    Price = 5.99m,
                    DiscountPercentage = 0,
                    StockQuantity = 200,
                    AverageRating = 4.8m,
                    CreatedAt = DateTime.UtcNow.AddDays(-23),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store2.StoreId,
                    CategoryId = groceriesCategory.CategoryId,
                    ProductName = "Fresh Bread Loaf",
                    Description = "Freshly baked whole wheat bread",
                    SKU = "FM-BRD-002",
                    Price = 3.49m,
                    DiscountPercentage = 10,
                    StockQuantity = 150,
                    AverageRating = 4.7m,
                    CreatedAt = DateTime.UtcNow.AddDays(-22),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store2.StoreId,
                    CategoryId = groceriesCategory.CategoryId,
                    ProductName = "Organic Eggs (12 count)",
                    Description = "Free-range organic eggs",
                    SKU = "FM-EGG-003",
                    Price = 6.99m,
                    DiscountPercentage = 5,
                    StockQuantity = 180,
                    AverageRating = 4.9m,
                    CreatedAt = DateTime.UtcNow.AddDays(-21),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store2.StoreId,
                    CategoryId = groceriesCategory.CategoryId,
                    ProductName = "Fresh Vegetables Bundle",
                    Description = "Mixed seasonal vegetables (2 lbs)",
                    SKU = "FM-VEG-004",
                    Price = 8.99m,
                    DiscountPercentage = 15,
                    StockQuantity = 120,
                    AverageRating = 4.6m,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store2.StoreId,
                    CategoryId = groceriesCategory.CategoryId,
                    ProductName = "Premium Coffee Beans (1 lb)",
                    Description = "Arabica coffee beans, medium roast",
                    SKU = "FM-COF-005",
                    Price = 12.99m,
                    DiscountPercentage = 20,
                    StockQuantity = 90,
                    AverageRating = 4.8m,
                    CreatedAt = DateTime.UtcNow.AddDays(-19),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Seed Products for Store 3 (Fashion Hub)
            var clothingCategory = categories.First(c => c.CategoryName == "Clothing");
            var products3 = new List<Product>
            {
                new Product
                {
                    StoreId = store3.StoreId,
                    CategoryId = clothingCategory.CategoryId,
                    ProductName = "Men's Cotton T-Shirt",
                    Description = "Comfortable cotton t-shirt, available in multiple colors",
                    SKU = "FH-TSH-001",
                    Price = 19.99m,
                    DiscountPercentage = 30,
                    StockQuantity = 200,
                    AverageRating = 4.4m,
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store3.StoreId,
                    CategoryId = clothingCategory.CategoryId,
                    ProductName = "Women's Denim Jeans",
                    Description = "Stylish slim-fit denim jeans",
                    SKU = "FH-JNS-002",
                    Price = 49.99m,
                    DiscountPercentage = 25,
                    StockQuantity = 150,
                    AverageRating = 4.5m,
                    CreatedAt = DateTime.UtcNow.AddDays(-17),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store3.StoreId,
                    CategoryId = clothingCategory.CategoryId,
                    ProductName = "Casual Sneakers",
                    Description = "Comfortable everyday sneakers",
                    SKU = "FH-SNK-003",
                    Price = 59.99m,
                    DiscountPercentage = 15,
                    StockQuantity = 100,
                    AverageRating = 4.3m,
                    CreatedAt = DateTime.UtcNow.AddDays(-16),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store3.StoreId,
                    CategoryId = clothingCategory.CategoryId,
                    ProductName = "Winter Jacket",
                    Description = "Warm and stylish winter jacket",
                    SKU = "FH-JKT-004",
                    Price = 89.99m,
                    DiscountPercentage = 35,
                    StockQuantity = 80,
                    AverageRating = 4.6m,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Seed Products for Store 4 (Book Nook)
            var booksCategory = categories.First(c => c.CategoryName == "Books");
            var products4 = new List<Product>
            {
                new Product
                {
                    StoreId = store4.StoreId,
                    CategoryId = booksCategory.CategoryId,
                    ProductName = "The Great Novel",
                    Description = "Bestselling fiction novel",
                    SKU = "BN-FIC-001",
                    Price = 14.99m,
                    DiscountPercentage = 20,
                    StockQuantity = 120,
                    AverageRating = 4.7m,
                    CreatedAt = DateTime.UtcNow.AddDays(-14),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store4.StoreId,
                    CategoryId = booksCategory.CategoryId,
                    ProductName = "Programming Guide",
                    Description = "Comprehensive programming tutorial book",
                    SKU = "BN-TEC-002",
                    Price = 39.99m,
                    DiscountPercentage = 15,
                    StockQuantity = 80,
                    AverageRating = 4.9m,
                    CreatedAt = DateTime.UtcNow.AddDays(-13),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store4.StoreId,
                    CategoryId = booksCategory.CategoryId,
                    ProductName = "Notebook Set (3 pack)",
                    Description = "High-quality ruled notebooks",
                    SKU = "BN-NTB-003",
                    Price = 9.99m,
                    DiscountPercentage = 0,
                    StockQuantity = 200,
                    AverageRating = 4.5m,
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    StoreId = store4.StoreId,
                    CategoryId = booksCategory.CategoryId,
                    ProductName = "Children's Story Book",
                    Description = "Illustrated children's adventure story",
                    SKU = "BN-CHD-004",
                    Price = 12.99m,
                    DiscountPercentage = 10,
                    StockQuantity = 150,
                    AverageRating = 4.8m,
                    CreatedAt = DateTime.UtcNow.AddDays(-11),
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Products.AddRangeAsync(products1);
            await context.Products.AddRangeAsync(products2);
            await context.Products.AddRangeAsync(products3);
            await context.Products.AddRangeAsync(products4);
            await context.SaveChangesAsync();

            // Seed Orders
            var allProducts = products1.Concat(products2).Concat(products3).Concat(products4).ToList();

            // Order 1: Customer 1 orders from Store 1 (Delivered)
            var order1 = new Order
            {
                CustomerId = customer1.Id,
                StoreId = store1.StoreId,
                OrderNumber = "ORD-20260201-120000-1001",
                DeliveryAddress = customer1.Address,
                SubTotal = 239.97m,
                DeliveryCharge = 5.99m,
                TotalAmount = 245.96m,
                Status = OrderStatus.Delivered,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };

            order1.OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = products1[0].ProductId,
                    ProductName = products1[0].ProductName,
                    Quantity = 2,
                    UnitPrice = 79.99m,
                    DiscountPercentage = 15,
                    LineTotal = 135.98m
                },
                new OrderItem
                {
                    ProductId = products1[2].ProductId,
                    ProductName = products1[2].ProductName,
                    Quantity = 1,
                    UnitPrice = 29.99m,
                    DiscountPercentage = 10,
                    LineTotal = 26.99m
                },
                new OrderItem
                {
                    ProductId = products1[4].ProductId,
                    ProductName = products1[4].ProductName,
                    Quantity = 1,
                    UnitPrice = 39.99m,
                    DiscountPercentage = 25,
                    LineTotal = 29.99m
                }
            };

            // Order 2: Customer 2 orders from Store 2 (Delivered)
            var order2 = new Order
            {
                CustomerId = customer2.Id,
                StoreId = store2.StoreId,
                OrderNumber = "ORD-20260205-140000-1002",
                DeliveryAddress = customer2.Address,
                SubTotal = 35.45m,
                DeliveryCharge = 3.99m,
                TotalAmount = 39.44m,
                Status = OrderStatus.Delivered,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            };

            order2.OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = products2[0].ProductId,
                    ProductName = products2[0].ProductName,
                    Quantity = 2,
                    UnitPrice = 5.99m,
                    DiscountPercentage = 0,
                    LineTotal = 11.98m
                },
                new OrderItem
                {
                    ProductId = products2[1].ProductId,
                    ProductName = products2[1].ProductName,
                    Quantity = 3,
                    UnitPrice = 3.49m,
                    DiscountPercentage = 10,
                    LineTotal = 9.42m
                },
                new OrderItem
                {
                    ProductId = products2[2].ProductId,
                    ProductName = products2[2].ProductName,
                    Quantity = 2,
                    UnitPrice = 6.99m,
                    DiscountPercentage = 5,
                    LineTotal = 13.28m
                }
            };

            // Order 3: Customer 3 orders from Store 3 (Accepted)
            var order3 = new Order
            {
                CustomerId = customer3.Id,
                StoreId = store3.StoreId,
                OrderNumber = "ORD-20260210-160000-1003",
                DeliveryAddress = customer3.Address,
                SubTotal = 97.48m,
                DeliveryCharge = 4.99m,
                TotalAmount = 102.47m,
                Status = OrderStatus.Accepted,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            };

            order3.OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = products3[0].ProductId,
                    ProductName = products3[0].ProductName,
                    Quantity = 3,
                    UnitPrice = 19.99m,
                    DiscountPercentage = 30,
                    LineTotal = 41.98m
                },
                new OrderItem
                {
                    ProductId = products3[2].ProductId,
                    ProductName = products3[2].ProductName,
                    Quantity = 1,
                    UnitPrice = 59.99m,
                    DiscountPercentage = 15,
                    LineTotal = 50.99m
                }
            };

            // Order 4: Customer 1 orders from Store 4 (Pending)
            var order4 = new Order
            {
                CustomerId = customer1.Id,
                StoreId = store4.StoreId,
                OrderNumber = "ORD-20260215-180000-1004",
                DeliveryAddress = customer1.Address,
                SubTotal = 65.96m,
                DeliveryCharge = 2.99m,
                TotalAmount = 68.95m,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            };

            order4.OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = products4[0].ProductId,
                    ProductName = products4[0].ProductName,
                    Quantity = 2,
                    UnitPrice = 14.99m,
                    DiscountPercentage = 20,
                    LineTotal = 23.98m
                },
                new OrderItem
                {
                    ProductId = products4[1].ProductId,
                    ProductName = products4[1].ProductName,
                    Quantity = 1,
                    UnitPrice = 39.99m,
                    DiscountPercentage = 15,
                    LineTotal = 33.99m
                },
                new OrderItem
                {
                    ProductId = products4[2].ProductId,
                    ProductName = products4[2].ProductName,
                    Quantity = 1,
                    UnitPrice = 9.99m,
                    DiscountPercentage = 0,
                    LineTotal = 9.99m
                }
            };

            await context.Orders.AddRangeAsync(new[] { order1, order2, order3, order4 });
            await context.SaveChangesAsync();

            // Seed Payments
            var payments = new List<Payment>
            {
                new Payment
                {
                    OrderId = order1.OrderId,
                    Amount = order1.TotalAmount,
                    PaymentMethod = PaymentMethod.CashOnDelivery,
                    Status = PaymentStatus.Completed,
                    CreatedAt = order1.CreatedAt,
                    UpdatedAt = order1.UpdatedAt
                },
                new Payment
                {
                    OrderId = order2.OrderId,
                    Amount = order2.TotalAmount,
                    PaymentMethod = PaymentMethod.CashOnDelivery,
                    Status = PaymentStatus.Completed,
                    CreatedAt = order2.CreatedAt,
                    UpdatedAt = order2.UpdatedAt
                },
                new Payment
                {
                    OrderId = order3.OrderId,
                    Amount = order3.TotalAmount,
                    PaymentMethod = PaymentMethod.CashOnDelivery,
                    Status = PaymentStatus.Pending,
                    CreatedAt = order3.CreatedAt,
                    UpdatedAt = order3.UpdatedAt
                },
                new Payment
                {
                    OrderId = order4.OrderId,
                    Amount = order4.TotalAmount,
                    PaymentMethod = PaymentMethod.CashOnDelivery,
                    Status = PaymentStatus.Pending,
                    CreatedAt = order4.CreatedAt,
                    UpdatedAt = order4.UpdatedAt
                }
            };

            await context.Payments.AddRangeAsync(payments);
            await context.SaveChangesAsync();

            // Seed Reviews for delivered orders
            var reviews = new List<Review>
            {
                // Store review for order 1
                new Review
                {
                    CustomerId = customer1.Id,
                    StoreId = store1.StoreId,
                    OrderId = order1.OrderId,
                    Rating = 5,
                    Comment = "Excellent service! Fast delivery and great products. Highly recommended!",
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                // Product reviews for order 1
                new Review
                {
                    CustomerId = customer1.Id,
                    ProductId = products1[0].ProductId,
                    OrderId = order1.OrderId,
                    Rating = 5,
                    Comment = "Amazing headphones! Sound quality is superb and battery life is as advertised.",
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                new Review
                {
                    CustomerId = customer1.Id,
                    ProductId = products1[4].ProductId,
                    OrderId = order1.OrderId,
                    Rating = 4,
                    Comment = "Good power bank, charges my phone quickly. Slightly heavy but worth it.",
                    CreatedAt = DateTime.UtcNow.AddDays(-4)
                },
                // Store review for order 2
                new Review
                {
                    CustomerId = customer2.Id,
                    StoreId = store2.StoreId,
                    OrderId = order2.OrderId,
                    Rating = 5,
                    Comment = "Fresh products and quick delivery. Will definitely order again!",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                // Product reviews for order 2
                new Review
                {
                    CustomerId = customer2.Id,
                    ProductId = products2[0].ProductId,
                    OrderId = order2.OrderId,
                    Rating = 5,
                    Comment = "Very fresh milk, tastes great! Delivery was on time.",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new Review
                {
                    CustomerId = customer2.Id,
                    ProductId = products2[2].ProductId,
                    OrderId = order2.OrderId,
                    Rating = 5,
                    Comment = "Best organic eggs I've had! Worth the price.",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };

            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();

            // Update product stock based on orders
            products1[0].StockQuantity -= 2;
            products1[2].StockQuantity -= 1;
            products1[4].StockQuantity -= 1;
            products2[0].StockQuantity -= 2;
            products2[1].StockQuantity -= 3;
            products2[2].StockQuantity -= 2;
            products3[0].StockQuantity -= 3;
            products3[2].StockQuantity -= 1;
            products4[0].StockQuantity -= 2;
            products4[1].StockQuantity -= 1;
            products4[2].StockQuantity -= 1;

            await context.SaveChangesAsync();
        }
    }
}
