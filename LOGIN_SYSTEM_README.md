# Login System with Role-Based Access Control

## Overview
A complete authentication and authorization system with role-based permissions using JSON file storage.

## Features
- Cookie-based authentication
- Role-based access control (RBAC)
- Permission-based page access
- Custom authorization attributes
- Access denied handling

## User Credentials

### Admin User
- **Username:** admin
- **Password:** Admin@123
- **Role:** Admin
- **Permissions:**
  - ViewDashboard
  - ManageUsers
  - ViewReports
  - EditData
  - DeleteData
  - ViewSettings

### Normal User
- **Username:** user
- **Password:** User@123
- **Role:** NormalUser
- **Permissions:**
  - ViewDashboard
  - ViewReports

### Data Entry Operator
- **Username:** dataentry
- **Password:** Data@123
- **Role:** DataEntryOperator
- **Permissions:**
  - ViewDashboard
  - EditData
  - ViewReports

## Available Pages

### Public Pages
- `/Account/Login` - Login page (accessible to all)

### Protected Pages (Require Authentication)
- `/Dashboard/Index` - Main dashboard (all authenticated users)
- `/Reports/Index` - Reports page (requires "ViewReports" permission)
- `/DataEntry/Index` - Data entry page (requires "EditData" permission)
- `/Admin/ManageUsers` - User management (requires "ManageUsers" permission)
- `/Admin/Settings` - System settings (requires "ViewSettings" permission)

## How It Works

### Authentication Flow
1. User enters credentials on login page
2. System validates against `Data/users.json`
3. On success, creates authentication cookie with claims
4. User is redirected to dashboard

### Authorization Flow
1. User attempts to access protected page
2. `[Authorize]` attribute checks if user is authenticated
3. `[Permission("PermissionName")]` attribute checks specific permission
4. If permission missing, redirects to Access Denied page

## Technical Implementation

### Key Components

**Models:**
- `User.cs` - User data model
- `LoginViewModel.cs` - Login form model

**Services:**
- `IAuthService` / `AuthService` - Handles authentication logic
- Reads user data from JSON file
- Validates credentials

**Controllers:**
- `AccountController` - Login/Logout/Access Denied
- `DashboardController` - Main dashboard
- `AdminController` - Admin-only pages
- `DataEntryController` - Data entry pages
- `ReportsController` - Reports pages

**Attributes:**
- `PermissionAttribute` - Custom authorization filter
- Checks user claims for specific permissions

**Data Storage:**
- `Data/users.json` - User credentials and permissions

## Adding New Users

Edit `Data/users.json`:

```json
{
  "users": [
    {
      "username": "newuser",
      "password": "Password@123",
      "role": "CustomRole",
      "permissions": ["ViewDashboard", "CustomPermission"]
    }
  ]
}
```

## Adding New Permissions

1. Add permission to user in `users.json`
2. Create controller action with `[Permission("PermissionName")]` attribute
3. Update dashboard to show link based on permission

Example:
```csharp
[Authorize]
public class MyController : Controller
{
    [Permission("MyCustomPermission")]
    public IActionResult MyAction()
    {
        return View();
    }
}
```

## Security Notes

⚠️ **Important:** This is a demo implementation. For production:
- Hash passwords (use BCrypt, Argon2, or PBKDF2)
- Use database instead of JSON file
- Implement account lockout
- Add two-factor authentication
- Use HTTPS only
- Implement CSRF protection (already included via ValidateAntiForgeryToken)
- Add password complexity requirements
- Implement session timeout
- Add audit logging

## Session Configuration

Cookie settings in `Program.cs`:
- **Expiration:** 8 hours
- **Sliding expiration:** Enabled
- **Login path:** /Account/Login
- **Access denied path:** /Account/AccessDenied

## Testing the System

1. Navigate to http://localhost:5264
2. Try logging in with different users
3. Observe different permissions and accessible pages
4. Try accessing restricted pages directly
5. Verify access denied redirects work correctly

## Customization

### Change Session Timeout
Edit `Program.cs`:
```csharp
options.ExpireTimeSpan = TimeSpan.FromHours(8); // Change duration
```

### Add New Role
1. Add user with new role in `users.json`
2. Assign appropriate permissions
3. No code changes needed!

### Modify Login Page Design
Edit `Views/Account/Login.cshtml`
