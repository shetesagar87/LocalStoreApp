# Authentication Test Steps

## Issue Fixed
Changed `CookieSecurePolicy.Always` to `CookieSecurePolicy.SameAsRequest` in Program.cs because:
- `Always` requires HTTPS
- Your app runs on HTTP (localhost:5264)
- This was preventing authentication cookies from being set

## Steps to Test Authentication

### 1. Stop and Restart the Application
**IMPORTANT:** You must restart the app for changes to take effect.

```powershell
# Stop the running app (Ctrl+C in terminal)
# Then restart:
dotnet run
```

### 2. Clear Browser Cache and Cookies
Before testing, clear your browser data:
- **Chrome/Edge:** Press `Ctrl+Shift+Delete` → Clear cookies and cached images
- **Or use Incognito/Private mode** for clean testing

### 3. Test Unauthenticated Access

Open browser and try to access:
```
http://localhost:5264/Home/Index
```

**Expected Result:** Should redirect to `http://localhost:5264/Account/Login`

### 4. Test Other Protected Pages

Try these URLs without logging in:
- `http://localhost:5264/Dashboard/Index` → Should redirect to Login
- `http://localhost:5264/Home/Privacy` → Should redirect to Login
- `http://localhost:5264/Admin/ManageUsers` → Should redirect to Login

### 5. Test Login Flow

1. Go to `http://localhost:5264/Account/Login`
2. Login with seeded admin account:
   - **Email:** admin@localstore.com
   - **Password:** Admin@123
3. After successful login, you should be redirected to `/Home/Index`

### 6. Test Authenticated Access

After logging in, try accessing:
- `http://localhost:5264/Home/Index` → Should work (show home page)
- `http://localhost:5264/Dashboard/Index` → Should work (show dashboard)
- `http://localhost:5264/Home/Privacy` → Should work (show privacy page)

### 7. Test Logout

1. Navigate to logout (or POST to `/Account/Logout`)
2. Try accessing `http://localhost:5264/Home/Index` again
3. Should redirect back to Login page

## Troubleshooting

### If authentication still doesn't work:

#### Check 1: Verify the app restarted
```powershell
# Look for this in console output:
[INF] Now listening on: http://localhost:5264
```

#### Check 2: Check browser console for errors
- Press F12 in browser
- Look for cookie-related errors

#### Check 3: Verify cookies are being set
After login:
1. Press F12 → Application tab (Chrome) or Storage tab (Firefox)
2. Look under Cookies → http://localhost:5264
3. Should see `.AspNetCore.Identity.Application` cookie

#### Check 4: Check application logs
```powershell
# View logs
Get-Content logs/log-*.txt -Tail 50
```

Look for:
- "User [email] logged in successfully"
- Any authentication errors

#### Check 5: Verify database is set up
```powershell
dotnet ef database update
```

Make sure AspNetUsers table exists and has the admin user.

## What Changed

### Program.cs
```csharp
// BEFORE (didn't work with HTTP)
options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

// AFTER (works with HTTP in development)
options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
```

### HomeController.cs
```csharp
[Authorize]  // ← Added this attribute
public class HomeController : Controller
{
    // All actions now require authentication
    // except Error which has [AllowAnonymous]
}
```

## Production Deployment Note

When deploying to production with HTTPS, consider changing back to:
```csharp
options.Cookie.SecurePolicy = app.Environment.IsDevelopment() 
    ? CookieSecurePolicy.SameAsRequest 
    : CookieSecurePolicy.Always;
```

This ensures cookies are secure in production while allowing HTTP in development.
