# Store Management Guide

## User Roles and Registration

### 1. Admin User (Pre-seeded)
The system automatically creates an admin user when the database is initialized.

**Admin Credentials:**
- Email: `admin@localstore.com`
- Password: `Admin@123`

**Admin Capabilities:**
- View all stores
- Approve/reject pending store registrations
- Manage users
- Access admin dashboard

### 2. Store Owner Registration
To register as a Store Owner:

1. Go to the registration page: `http://localhost:5264/Account/Register`
2. Fill in all required fields:
   - Full Name
   - Email
   - Phone Number
   - Address
   - Password (minimum 8 characters)
   - Confirm Password
3. **Check the "Register as Store Owner" checkbox** ✓
4. Click "Register"
5. Verify your email (check logs for verification link if email is not configured)

**Store Owner Capabilities:**
- Register new stores
- View and manage their stores
- Edit store details
- Toggle store active/inactive status
- Upload store images

### 3. Customer Registration
To register as a Customer:

1. Go to the registration page: `http://localhost:5264/Account/Register`
2. Fill in all required fields
3. **Leave the "Register as Store Owner" checkbox unchecked**
4. Click "Register"
5. Verify your email

**Customer Capabilities:**
- Browse stores and products
- Place orders
- Write reviews
- Manage shopping cart

## Store Registration Workflow

### Step 1: Store Owner Registers a Store
1. Login as a Store Owner
2. Click "My Stores" in the navigation bar
3. Click "Register New Store"
4. Fill in store details:
   - Store Name
   - Store Address
   - Latitude and Longitude (for geo-location)
   - Business License Number
   - Delivery Radius (in km)
   - Delivery Charge
   - Store Image (optional)
5. Submit the form
6. Store is created with status: **Pending**

### Step 2: Admin Approves the Store
1. Login as Admin
2. Click "Admin" → "Pending Stores" in the navigation
3. View the pending store details
4. Click "Approve" to approve the store
   - OR -
5. Click "Reject" and provide a reason to reject the store

### Step 3: Store Owner Manages the Store
After approval:
1. Store status changes to: **Approved**
2. Store becomes **Active** automatically
3. Store Owner can:
   - View store details
   - Edit store information
   - Toggle store active/inactive status
   - Add products (coming in next tasks)

## Navigation Links

### For Store Owners:
- **My Stores** - View and manage your stores

### For Admins:
- **Admin** dropdown menu:
  - **Pending Stores** - Approve/reject store registrations
  - **All Stores** - View all registered stores
  - **Manage Users** - User management (coming soon)
  - **Settings** - System settings (coming soon)

## Testing the Store Management System

### Quick Test Scenario:

1. **Login as Admin:**
   - Email: `admin@localstore.com`
   - Password: `Admin@123`
   - Verify admin navigation appears

2. **Register a Store Owner:**
   - Logout
   - Register new user with "Register as Store Owner" checked
   - Login with the new Store Owner account

3. **Create a Store:**
   - Click "My Stores"
   - Click "Register New Store"
   - Fill in store details (use sample coordinates like: Lat: 40.7128, Lon: -74.0060)
   - Submit

4. **Approve the Store:**
   - Logout
   - Login as Admin
   - Click "Admin" → "Pending Stores"
   - Click "Approve" on the pending store

5. **Verify Store is Active:**
   - Logout
   - Login as Store Owner
   - Click "My Stores"
   - Verify store shows "Approved" and "Active" status

## Troubleshooting

### "My Stores" link not visible
- Ensure you registered with "Register as Store Owner" checkbox checked
- Logout and login again to refresh the session

### Admin menu not visible
- Use the pre-seeded admin account: `admin@localstore.com` / `Admin@123`
- Regular users cannot become admins through registration

### Email verification required
- Check the application logs for the verification link
- Or manually set `EmailConfirmed = true` in the database for testing

### Store images not uploading
- Ensure `wwwroot/images/stores` directory exists (created automatically)
- Check file size and format (images only)

## Next Steps

After completing store management, the next features to implement are:
- Product catalog management (Task 8)
- Product search and browsing (Task 9)
- Shopping cart (Task 11)
- Checkout and orders (Task 12)
