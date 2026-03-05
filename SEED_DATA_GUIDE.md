# Seed Data Guide

## Overview
This document describes the comprehensive demo data that is automatically seeded into the database when you first run the application.

## Automatic Seeding
The seed data is automatically created when you:
1. Run the application for the first time
2. Have an empty database (no existing categories or stores)

The seeding happens in two phases:
- **Phase 1**: Users and roles (always runs)
- **Phase 2**: Demo data (only if database is empty)

---

## 📊 Seeded Data Summary

### Users (6 total)
- 1 Admin
- 2 Store Owners
- 3 Customers

### Categories (8 total)
- Electronics
- Groceries
- Clothing
- Books
- Home & Kitchen
- Sports
- Toys
- Beauty

### Stores (4 total)
- 2 stores owned by Owner 1
- 2 stores owned by Owner 2
- All stores are approved and active

### Products (18 total)
- 5 products in Tech Haven Electronics
- 5 products in Fresh Mart Groceries
- 4 products in Fashion Hub
- 4 products in Book Nook

### Orders (4 total)
- 2 delivered orders (with reviews)
- 1 accepted order (in progress)
- 1 pending order (awaiting store approval)

### Reviews (6 total)
- 2 store reviews
- 4 product reviews
- All from delivered orders

---

## 👥 User Accounts

### Admin Account
```
Email: admin@localstore.com
Password: Admin@123
Role: Admin
Full Name: System Administrator
Address: 123 Admin Street, City Center
Phone: +1234567890
```

**Capabilities:**
- View admin dashboard with analytics
- Approve/reject store registrations
- Manage categories
- Manage users (enable/disable)
- View all orders and stores
- Delete inappropriate reviews

### Store Owner 1
```
Email: owner1@localstore.com
Password: Owner@123
Role: StoreOwner
Full Name: John Smith
Address: 456 Market Street, Downtown
Phone: +1234567891
```

**Owns:**
- Tech Haven Electronics
- Fresh Mart Groceries

### Store Owner 2
```
Email: owner2@localstore.com
Password: Owner@123
Role: StoreOwner
Full Name: Sarah Johnson
Address: 789 Commerce Ave, Business District
Phone: +1234567892
```

**Owns:**
- Fashion Hub
- Book Nook

### Customer 1
```
Email: customer1@localstore.com
Password: Customer@123
Role: Customer
Full Name: Michael Brown
Address: 321 Residential Lane, Suburb Area
Phone: +1234567893
```

**Order History:**
- 1 delivered order from Tech Haven (with reviews)
- 1 pending order from Book Nook

### Customer 2
```
Email: customer2@localstore.com
Password: Customer@123
Role: Customer
Full Name: Emily Davis
Address: 654 Park View, Green Valley
Phone: +1234567894
```

**Order History:**
- 1 delivered order from Fresh Mart (with reviews)

### Customer 3
```
Email: customer3@localstore.com
Password: Customer@123
Role: Customer
Full Name: David Wilson
Address: 987 Lake Side Drive, Waterfront
Phone: +1234567895
```

**Order History:**
- 1 accepted order from Fashion Hub (in progress)

---

## 🏪 Stores

### 1. Tech Haven Electronics
- **Owner**: John Smith (owner1@localstore.com)
- **Address**: 123 Tech Street, Silicon Valley, CA 94025
- **License**: LIC-2024-001
- **Delivery Radius**: 10 km
- **Hours**: 9:00 AM - 9:00 PM
- **Delivery Charge**: $5.99
- **Rating**: 4.5/5
- **Status**: Approved & Active
- **Products**: 5 electronics items

### 2. Fresh Mart Groceries
- **Owner**: John Smith (owner1@localstore.com)
- **Address**: 456 Market Avenue, Downtown, CA 94102
- **License**: LIC-2024-002
- **Delivery Radius**: 8 km
- **Hours**: 7:00 AM - 10:00 PM
- **Delivery Charge**: $3.99
- **Rating**: 4.7/5
- **Status**: Approved & Active
- **Products**: 5 grocery items

### 3. Fashion Hub
- **Owner**: Sarah Johnson (owner2@localstore.com)
- **Address**: 789 Style Boulevard, Fashion District, CA 90015
- **License**: LIC-2024-003
- **Delivery Radius**: 12 km
- **Hours**: 10:00 AM - 8:00 PM
- **Delivery Charge**: $4.99
- **Rating**: 4.3/5
- **Status**: Approved & Active
- **Products**: 4 clothing items

### 4. Book Nook
- **Owner**: Sarah Johnson (owner2@localstore.com)
- **Address**: 321 Library Lane, University Area, CA 94720
- **License**: LIC-2024-004
- **Delivery Radius**: 7 km
- **Hours**: 8:00 AM - 7:00 PM
- **Delivery Charge**: $2.99
- **Rating**: 4.8/5
- **Status**: Approved & Active
- **Products**: 4 book items

---

## 📦 Products by Store

### Tech Haven Electronics (5 products)

1. **Wireless Bluetooth Headphones**
   - Price: $79.99 (15% off = $67.99)
   - Stock: 48 units (2 sold)
   - Rating: 4.6/5
   - SKU: TH-WH-001

2. **Smart Watch Pro**
   - Price: $199.99 (20% off = $159.99)
   - Stock: 30 units
   - Rating: 4.7/5
   - SKU: TH-SW-002

3. **USB-C Fast Charger**
   - Price: $29.99 (10% off = $26.99)
   - Stock: 99 units (1 sold)
   - Rating: 4.4/5
   - SKU: TH-CH-003

4. **Wireless Mouse**
   - Price: $24.99 (no discount)
   - Stock: 75 units
   - Rating: 4.3/5
   - SKU: TH-MS-004

5. **Portable Power Bank 20000mAh**
   - Price: $39.99 (25% off = $29.99)
   - Stock: 59 units (1 sold)
   - Rating: 4.5/5
   - SKU: TH-PB-005

### Fresh Mart Groceries (5 products)

1. **Organic Fresh Milk (1 Gallon)**
   - Price: $5.99 (no discount)
   - Stock: 198 units (2 sold)
   - Rating: 4.8/5
   - SKU: FM-MLK-001

2. **Fresh Bread Loaf**
   - Price: $3.49 (10% off = $3.14)
   - Stock: 147 units (3 sold)
   - Rating: 4.7/5
   - SKU: FM-BRD-002

3. **Organic Eggs (12 count)**
   - Price: $6.99 (5% off = $6.64)
   - Stock: 178 units (2 sold)
   - Rating: 4.9/5
   - SKU: FM-EGG-003

4. **Fresh Vegetables Bundle**
   - Price: $8.99 (15% off = $7.64)
   - Stock: 120 units
   - Rating: 4.6/5
   - SKU: FM-VEG-004

5. **Premium Coffee Beans (1 lb)**
   - Price: $12.99 (20% off = $10.39)
   - Stock: 90 units
   - Rating: 4.8/5
   - SKU: FM-COF-005

### Fashion Hub (4 products)

1. **Men's Cotton T-Shirt**
   - Price: $19.99 (30% off = $13.99)
   - Stock: 197 units (3 sold)
   - Rating: 4.4/5
   - SKU: FH-TSH-001

2. **Women's Denim Jeans**
   - Price: $49.99 (25% off = $37.49)
   - Stock: 150 units
   - Rating: 4.5/5
   - SKU: FH-JNS-002

3. **Casual Sneakers**
   - Price: $59.99 (15% off = $50.99)
   - Stock: 99 units (1 sold)
   - Rating: 4.3/5
   - SKU: FH-SNK-003

4. **Winter Jacket**
   - Price: $89.99 (35% off = $58.49)
   - Stock: 80 units
   - Rating: 4.6/5
   - SKU: FH-JKT-004

### Book Nook (4 products)

1. **The Great Novel**
   - Price: $14.99 (20% off = $11.99)
   - Stock: 118 units (2 sold)
   - Rating: 4.7/5
   - SKU: BN-FIC-001

2. **Programming Guide**
   - Price: $39.99 (15% off = $33.99)
   - Stock: 79 units (1 sold)
   - Rating: 4.9/5
   - SKU: BN-TEC-002

3. **Notebook Set (3 pack)**
   - Price: $9.99 (no discount)
   - Stock: 199 units (1 sold)
   - Rating: 4.5/5
   - SKU: BN-NTB-003

4. **Children's Story Book**
   - Price: $12.99 (10% off = $11.69)
   - Stock: 150 units
   - Rating: 4.8/5
   - SKU: BN-CHD-004

---

## 🛒 Orders

### Order 1: ORD-20260201-120000-1001
- **Customer**: Michael Brown (customer1@localstore.com)
- **Store**: Tech Haven Electronics
- **Status**: Delivered ✅
- **Date**: 10 days ago → Delivered 5 days ago
- **Items**:
  - 2x Wireless Bluetooth Headphones = $135.98
  - 1x USB-C Fast Charger = $26.99
  - 1x Portable Power Bank = $29.99
- **Subtotal**: $239.97
- **Delivery**: $5.99
- **Total**: $245.96
- **Payment**: Cash on Delivery (Completed)
- **Reviews**: ✅ Store and 2 products reviewed

### Order 2: ORD-20260205-140000-1002
- **Customer**: Emily Davis (customer2@localstore.com)
- **Store**: Fresh Mart Groceries
- **Status**: Delivered ✅
- **Date**: 8 days ago → Delivered 3 days ago
- **Items**:
  - 2x Organic Fresh Milk = $11.98
  - 3x Fresh Bread Loaf = $9.42
  - 2x Organic Eggs = $13.28
- **Subtotal**: $35.45
- **Delivery**: $3.99
- **Total**: $39.44
- **Payment**: Cash on Delivery (Completed)
- **Reviews**: ✅ Store and 2 products reviewed

### Order 3: ORD-20260210-160000-1003
- **Customer**: David Wilson (customer3@localstore.com)
- **Store**: Fashion Hub
- **Status**: Accepted (In Progress) 🔄
- **Date**: 5 days ago → Accepted 4 days ago
- **Items**:
  - 3x Men's Cotton T-Shirt = $41.98
  - 1x Casual Sneakers = $50.99
- **Subtotal**: $97.48
- **Delivery**: $4.99
- **Total**: $102.47
- **Payment**: Cash on Delivery (Pending)
- **Reviews**: Not yet (order not delivered)

### Order 4: ORD-20260215-180000-1004
- **Customer**: Michael Brown (customer1@localstore.com)
- **Store**: Book Nook
- **Status**: Pending ⏳
- **Date**: 2 days ago
- **Items**:
  - 2x The Great Novel = $23.98
  - 1x Programming Guide = $33.99
  - 1x Notebook Set = $9.99
- **Subtotal**: $65.96
- **Delivery**: $2.99
- **Total**: $68.95
- **Payment**: Cash on Delivery (Pending)
- **Reviews**: Not yet (order not delivered)

---

## ⭐ Reviews

### Store Reviews (2)

1. **Tech Haven Electronics** - 5/5 stars
   - By: Michael Brown
   - Comment: "Excellent service! Fast delivery and great products. Highly recommended!"
   - Date: 4 days ago

2. **Fresh Mart Groceries** - 5/5 stars
   - By: Emily Davis
   - Comment: "Fresh products and quick delivery. Will definitely order again!"
   - Date: 2 days ago

### Product Reviews (4)

1. **Wireless Bluetooth Headphones** - 5/5 stars
   - By: Michael Brown
   - Comment: "Amazing headphones! Sound quality is superb and battery life is as advertised."

2. **Portable Power Bank** - 4/5 stars
   - By: Michael Brown
   - Comment: "Good power bank, charges my phone quickly. Slightly heavy but worth it."

3. **Organic Fresh Milk** - 5/5 stars
   - By: Emily Davis
   - Comment: "Very fresh milk, tastes great! Delivery was on time."

4. **Organic Eggs** - 5/5 stars
   - By: Emily Davis
   - Comment: "Best organic eggs I've had! Worth the price."

---

## 🎯 Testing Scenarios

### Scenario 1: Admin Workflow
1. Login as admin@localstore.com
2. View admin dashboard (see sales, stores, orders)
3. Check pending stores (none - all approved)
4. Manage categories
5. View and manage users
6. Check top-selling products

### Scenario 2: Store Owner Workflow
1. Login as owner1@localstore.com or owner2@localstore.com
2. View store dashboard (sales, pending orders, low stock)
3. Manage products (view existing products)
4. View orders (accept/reject pending orders)
5. Update order status (mark as preparing, out for delivery, delivered)

### Scenario 3: Customer Shopping Workflow
1. Login as customer1@localstore.com
2. Browse products by store
3. Search for products
4. Add items to cart
5. Checkout and place order
6. View order history
7. Review delivered orders

### Scenario 4: Complete Order Lifecycle
1. Customer places order (Pending)
2. Store owner accepts order (Accepted)
3. Store owner marks as preparing (Preparing)
4. Store owner marks as out for delivery (Out for Delivery)
5. Store owner marks as delivered (Delivered)
6. Customer leaves review

---

## 🔄 Re-seeding Data

If you want to reset and re-seed the data:

1. **Drop the database**:
   ```bash
   dotnet ef database drop -f
   ```

2. **Recreate and seed**:
   ```bash
   dotnet ef database update
   dotnet run
   ```

The seed data will be automatically created on the first run.

---

## 📝 Notes

- All users have pre-verified emails (no verification needed)
- All stores are pre-approved and active
- Product stock quantities reflect completed orders
- Reviews can only be left for delivered orders
- Payment method is Cash on Delivery for all orders
- Delivery charges vary by store
- Product discounts are already applied in the seed data

---

**Last Updated**: February 22, 2026  
**Seed Data Version**: 1.0  
**Total Records**: 6 users, 8 categories, 4 stores, 18 products, 4 orders, 6 reviews
