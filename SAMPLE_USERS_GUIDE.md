# Sample Users Guide

## Overview

The Local Store Platform comes pre-seeded with sample user accounts for testing all features. All sample users have their email already verified and are active.

## Password Format

All sample user passwords follow the same format: `Role@123`
- Admin: `Admin@123`
- Store Owners: `Owner@123`
- Customers: `Customer@123`

## Sample User Accounts

### 1. Admin Account

#### System Administrator
- **Email**: admin@localstore.com
- **Password**: Admin@123
- **Full Name**: System Administrator
- **Address**: 123 Admin Street, City Center
- **Phone**: +1234567890
- **Role**: Admin

**Capabilities**:
- Approve/reject store registrations
- Manage product categories
- View all stores and products
- Manage users
- Access admin dashboard
- Full system access

**How to Use**:
1. Navigate to http://localhost:5264
2. Login with admin@localstore.com / Admin@123
3. Access admin features from "Admin" dropdown in navigation

---

### 2. Store Owner Accounts

#### Store Owner 1 - John Smith
- **Email**: owner1@localstore.com
- **Password**: Owner@123
- **Full Name**: John Smith
- **Address**: 456 Market Street, Downtown
- **Phone**: +1234567891
- **Role**: StoreOwner

**Suggested Store**: Downtown Grocery
**Suggested Products**: Fresh produce, dairy, bakery items

#### Store Owner 2 - Sarah Johnson
- **Email**: owner2@localstore.com
- **Password**: Owner@123
- **Full Name**: Sarah Johnson
- **Address**: 789 Commerce Ave, Business District
- **Phone**: +1234567892
- **Role**: StoreOwner

**Suggested Store**: Tech Electronics Hub
**Suggested Products**: Smartphones, laptops, accessories

**Store Owner Capabilities**:
- Register and manage stores
- Add/edit/delete products
- Manage inventory and stock
- View store orders (when implemented)
- Update store status (active/inactive)
- Upload store and product images

**How to Use**:
1. Login with store owner credentials
2. Navigate to "My Stores"
3. Click "Register New Store"
4. Wait for admin approval
5. Once approved, add products to your store

---

### 3. Customer Accounts

#### Customer 1 - Michael Brown
- **Email**: customer1@localstore.com
- **Password**: Customer@123
- **Full Name**: Michael Brown
- **Address**: 321 Residential Lane, Suburb Area
- **Phone**: +1234567893
- **Role**: Customer

**Test Scenario**: Regular shopper, frequent orders

#### Customer 2 - Emily Davis
- **Email**: customer2@localstore.com
- **Password**: Customer@123
- **Full Name**: Emily Davis
- **Address**: 654 Park View, Green Valley
- **Phone**: +1234567894
- **Role**: Customer

**Test Scenario**: New customer, first-time buyer

#### Customer 3 - David Wilson
- **Email**: customer3@localstore.com
- **Password**: Customer@123
- **Full Name**: David Wilson
- **Address**: 987 Lake Side Drive, Waterfront
- **Phone**: +1234567895
- **Role**: Customer

**Test Scenario**: Bulk buyer, large orders

**Customer Capabilities**:
- Browse stores and products
- Search and filter products
- Add items to shopping cart
- Update cart quantities
- Place orders (when implemented)
- Track order status (when implemented)
- Write reviews (when implemented)

**How to Use**:
1. Login with customer credentials
2. Browse stores from home page
3. Click "Browse Products" on any store
4. Add items to cart
5. View cart and proceed to checkout

---

## Testing Workflows

### Complete Store Setup Workflow

1. **Admin Login** (admin@localstore.com)
   - Create product categories (e.g., Electronics, Groceries, Clothing)

2. **Store Owner Login** (owner1@localstore.com)
   - Register a new store
   - Fill in all store details
   - Upload store image

3. **Admin Login** (admin@localstore.com)
   - Go to "Admin" → "Pending Stores"
   - Review store details
   - Click "Approve"

4. **Store Owner Login** (owner1@localstore.com)
   - Go to "My Stores"
   - Click "Products" on approved store
   - Add multiple products with different categories
   - Upload product images
   - Set stock quantities

5. **Customer Login** (customer1@localstore.com)
   - Browse stores from home page
   - Search for products
   - View product details
   - Add items to cart
   - Update quantities in cart
   - Proceed to checkout (when implemented)

### Multi-Store Testing

1. **Store Owner 1** (owner1@localstore.com)
   - Create "Downtown Grocery" store
   - Add grocery products

2. **Store Owner 2** (owner2@localstore.com)
   - Create "Tech Electronics Hub" store
   - Add electronics products

3. **Admin** (admin@localstore.com)
   - Approve both stores

4. **Customer** (customer1@localstore.com)
   - Browse both stores
   - Add products from different stores to cart
   - Compare prices and delivery charges

### Cart and Shopping Testing

1. **Customer 1** (customer1@localstore.com)
   - Add 5 different products to cart
   - Update quantities
   - Remove some items
   - Clear cart
   - Add items again

2. **Customer 2** (customer2@localstore.com)
   - Add products from multiple stores
   - Check cart subtotal calculation
   - Verify stock validation

3. **Customer 3** (customer3@localstore.com)
   - Try adding out-of-stock items
   - Try adding quantity exceeding stock
   - Verify error messages

---

## Quick Login Reference

| Role | Email | Password | Use Case |
|------|-------|----------|----------|
| Admin | admin@localstore.com | Admin@123 | System management |
| Store Owner 1 | owner1@localstore.com | Owner@123 | Grocery store |
| Store Owner 2 | owner2@localstore.com | Owner@123 | Electronics store |
| Customer 1 | customer1@localstore.com | Customer@123 | Regular shopper |
| Customer 2 | customer2@localstore.com | Customer@123 | New customer |
| Customer 3 | customer3@localstore.com | Customer@123 | Bulk buyer |

---

## Creating Additional Test Users

### Via Registration Page

1. Navigate to http://localhost:5264/Account/Register
2. Fill in user details
3. For Store Owner: Check "Register as Store Owner"
4. For Customer: Leave checkbox unchecked
5. Click "Register"
6. Email is auto-verified in development mode

### Password Requirements

All passwords must meet these requirements:
- Minimum 8 characters
- At least one uppercase letter (A-Z)
- At least one lowercase letter (a-z)
- At least one digit (0-9)
- At least one special character (!@#$%^&*)

**Valid Examples**:
- `Password@123`
- `Test@User1`
- `MyStore#2024`

**Invalid Examples**:
- `password` (no uppercase, no digit, no special char)
- `Pass@1` (too short)
- `PASSWORD@123` (no lowercase)

---

## Troubleshooting

### Cannot Login
- Verify email and password are correct
- Check if account is active
- Ensure email is verified (auto-verified for seeded users)

### Store Owner Cannot Add Products
- Verify store is approved by admin
- Check if store is active
- Ensure you're logged in as the store owner

### Customer Cannot Add to Cart
- Check if product is in stock
- Verify you're logged in
- Ensure product is from an active store

### Admin Cannot See Pending Stores
- Verify you're logged in as admin
- Check if any stores are in pending status
- Refresh the page

---

## Database Reset

If you need to reset the database and re-seed users:

```bash
# Delete existing database
dotnet ef database drop --force

# Create new database with migrations
dotnet ef database update

# Run the application (seeding happens automatically)
dotnet run
```

All sample users will be recreated automatically.

---

## Security Notes

⚠️ **Important**: These are sample accounts for development and testing only.

**For Production**:
- Change all default passwords
- Remove or disable sample accounts
- Use strong, unique passwords
- Enable proper email verification
- Implement rate limiting
- Add two-factor authentication

---

## Next Steps

After testing with sample users:

1. **Implement Checkout** (Task 12)
   - Order placement
   - Address selection
   - Payment method selection

2. **Implement Payment** (Task 13)
   - Payment gateway integration
   - Payment status tracking
   - Refund processing

3. **Implement Order Management** (Task 14)
   - Order tracking
   - Status updates
   - Order history

4. **Implement Reviews** (Task 16)
   - Product reviews
   - Store reviews
   - Rating calculations

5. **Implement Notifications** (Task 17)
   - Email notifications
   - Order confirmations
   - Status updates

---

**Last Updated**: February 22, 2026
**Version**: 1.0
