# Quick Start Guide - Local Store Platform

## Application Status

✅ **Application is running successfully!**
- URL: http://localhost:5264
- Database: Connected and seeded
- Sample users: Created and ready to use

## Quick Login Credentials

### Admin
```
Email: admin@localstore.com
Password: Admin@123
```

### Store Owners
```
Email: owner1@localstore.com
Password: Owner@123
Name: John Smith

Email: owner2@localstore.com
Password: Owner@123
Name: Sarah Johnson
```

### Customers
```
Email: customer1@localstore.com
Password: Customer@123
Name: Michael Brown

Email: customer2@localstore.com
Password: Customer@123
Name: Emily Davis

Email: customer3@localstore.com
Password: Customer@123
Name: David Wilson
```

## 5-Minute Test Flow

### 1. Admin Setup (2 minutes)
1. Login as admin@localstore.com / Admin@123
2. Go to Admin → Categories
3. Create categories: Electronics, Groceries, Clothing
4. Logout

### 2. Store Owner Setup (2 minutes)
1. Login as owner1@localstore.com / Owner@123
2. Go to "My Stores" → "Register New Store"
3. Fill in:
   - Store Name: Downtown Grocery
   - Address: 123 Main St
   - License: LIC-001
   - Opening: 08:00, Closing: 20:00
   - Delivery Radius: 5 km
   - Delivery Charge: 50
4. Submit and logout

### 3. Admin Approval (30 seconds)
1. Login as admin@localstore.com / Admin@123
2. Go to Admin → Pending Stores
3. Click "Details" on Downtown Grocery
4. Click "Approve"
5. Logout

### 4. Add Products (1 minute)
1. Login as owner1@localstore.com / Owner@123
2. Go to "My Stores"
3. Click "Products" on Downtown Grocery
4. Click "Add New Product"
5. Fill in:
   - Name: Fresh Milk
   - SKU: MILK-001
   - Category: Groceries
   - Price: 50
   - Stock: 100
6. Submit
7. Add 2-3 more products
8. Logout

### 5. Customer Shopping (1 minute)
1. Login as customer1@localstore.com / Customer@123
2. Home page shows Downtown Grocery
3. Click "Browse Products"
4. Click "Add to Cart" on products
5. Click cart icon (top right)
6. View cart with items
7. Update quantities, remove items

## Current Features

✅ **Implemented**:
- User authentication (Admin, StoreOwner, Customer)
- Store registration and approval workflow
- Category management
- Product management (CRUD, stock, images)
- Product browsing and search
- Shopping cart (add, update, remove)
- Cart badge with item count

🚧 **Coming Next**:
- Checkout and order placement
- Payment processing
- Order management
- Reviews and ratings
- Notifications

## Key Navigation

### Admin Menu
- Admin → Pending Stores
- Admin → All Stores
- Admin → Categories

### Store Owner Menu
- My Stores
- My Stores → Products (per store)

### Customer Menu
- Home (browse stores)
- Browse Products (search all)
- Cart icon (top right)

## Troubleshooting

### Cannot Login
- Verify credentials are correct
- All sample users have verified emails

### Store Not Showing
- Admin must approve store first
- Store must be active

### Cannot Add to Cart
- Product must be in stock
- Must be logged in as customer

### Cart Badge Not Updating
- Refresh page after adding items
- Check browser console for errors

## Documentation

- `SAMPLE_USERS_GUIDE.md` - Complete user account details
- `STORE_MANAGEMENT_GUIDE.md` - Store management workflows
- `IMPLEMENTATION_STATUS.md` - Feature completion status
- `ERRORS_RESOLVED.md` - Recent fixes

## Database Commands

### Reset Database
```bash
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

### View Migrations
```bash
dotnet ef migrations list
```

### Create New Migration
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Development Commands

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run
```

### Watch (auto-reload)
```bash
dotnet watch run
```

## Application Logs

Logs are stored in: `logs/log-YYYYMMDD.txt`

Current log: `logs/log-20260222.txt`

## Next Steps

1. Test the complete flow above
2. Create more stores with owner2@localstore.com
3. Test with different customer accounts
4. Proceed with Task 12: Checkout implementation

---

**Application Version**: 1.0
**Last Updated**: February 22, 2026
**Status**: Development - Ready for Testing
