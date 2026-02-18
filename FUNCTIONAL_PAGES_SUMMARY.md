# Functional Pages Summary

## Authentication Flow

### Default Behavior
- **Default Route:** `/Account/Login`
- **Unauthenticated users** are automatically redirected to `/Account/Login`
- **Login Path:** Configured in `Program.cs` → `options.LoginPath = "/Account/Login"`

## Page Access Matrix

### 🔓 Public Pages (No Authentication Required)

| Route | Description | Status |
|-------|-------------|--------|
| `/Account/Login` | User login page | ✅ Functional |
| `/Account/Register` | New user registration | ✅ Functional |
| `/Account/AccessDenied` | Access denied message | ✅ Functional |
| `/Home/Error` | Error page | ✅ Functional |

### 🔒 Protected Pages (Authentication Required)

| Route | Description | Authorization | Status |
|-------|-------------|---------------|--------|
| `/Home/Index` | Home page | Login required | ✅ Protected |
| `/Home/Privacy` | Privacy policy | Login required | ✅ Protected |
| `/Dashboard/Index` | User dashboard | Login required | ✅ Functional |
| `/Admin/ManageUsers` | User management | Admin + ManageUsers permission | ✅ Functional |
| `/Admin/Settings` | System settings | Admin + ViewSettings permission | ✅ Functional |
| `/DataEntry/Index` | Data entry interface | Login + EditData permission | ✅ Functional |
| `/Reports/Index` | Reports viewer | Login + ViewReports permission | ✅ Functional |

## Authentication Configuration

### Cookie Settings
- **Expiration:** 8 hours
- **Sliding Expiration:** Enabled
- **Secure Policy:** Always (HTTPS only)
- **HttpOnly:** Enabled

### Identity Settings
- **Password Requirements:**
  - Minimum 8 characters
  - Requires digit
  - Requires lowercase
  - Requires uppercase
  - Requires non-alphanumeric character
  
- **Lockout Settings:**
  - Max failed attempts: 5
  - Lockout duration: 15 minutes
  
- **Email Verification:**
  - Required for sign-in: Yes

## User Flow

### New User Registration
1. Navigate to `/Account/Register`
2. Fill registration form (Name, Email, Phone, Address, Password)
3. Choose role: Customer or Store Owner
4. Submit → Email verification required
5. Verify email (currently TODO - email sending not implemented)
6. Login at `/Account/Login`

### Existing User Login
1. Navigate to `/Account/Login` (or any protected page - auto-redirects)
2. Enter email and password
3. Optional: Check "Remember me"
4. Submit → Redirected to `/Home/Index` or original requested page

### Access Denied
- If user tries to access a page without required permissions
- Redirected to `/Account/AccessDenied`

## Default Seeded Users

After database migration and seeding:

| Email | Password | Role | Permissions |
|-------|----------|------|-------------|
| admin@localstore.com | Admin@123 | Admin | All permissions |

## Next Steps (Not Yet Implemented)

### Store Management (Task 6)
- `/Store/Create` - Register new store
- `/Store/Edit/{id}` - Edit store details
- `/Store/Details/{id}` - View store details
- `/Admin/Store/Approve/{id}` - Approve pending stores

### Product Management (Task 8)
- `/Product/Create` - Add new product
- `/Product/Edit/{id}` - Edit product
- `/Product/List` - Browse products
- `/Product/Details/{id}` - Product details

### Shopping & Orders (Tasks 11-12)
- `/Cart/View` - Shopping cart
- `/Checkout` - Checkout process
- `/Order/MyOrders` - Customer orders
- `/Order/Details/{id}` - Order details

### Reviews & Ratings (Task 16)
- `/Review/Create` - Submit review
- `/Review/List` - View reviews

## Testing the Authentication

### Test Unauthenticated Access
1. Open browser in incognito/private mode
2. Navigate to `http://localhost:5264/Home/Index`
3. Should redirect to `/Account/Login`

### Test Authenticated Access
1. Login with admin credentials
2. Navigate to `http://localhost:5264/Dashboard/Index`
3. Should display dashboard

### Test Permission-Based Access
1. Login as regular user (after creating one)
2. Try to access `/Admin/ManageUsers`
3. Should redirect to `/Account/AccessDenied`
