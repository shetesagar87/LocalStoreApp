# Database Setup Instructions

## Issue Summary
The database tables haven't been created yet. The migrations exist but haven't been applied to the SQL Server database.

## Steps to Fix

### 1. Stop the Running Application
The application is currently running (Process ID: 23308). You need to stop it first:
- Press `Ctrl+C` in the terminal where the app is running
- OR close the browser and wait a few seconds
- OR use Task Manager to end the `CleanMvcApp.exe` process

### 2. Apply Database Migrations
Once the app is stopped, run this command:

```powershell
dotnet ef database update
```

This will:
- Create the `CleanMvcAppDb` database on your SQL Server (SSHETEX64VM)
- Create all required tables (AspNetUsers, AspNetRoles, Stores, Products, Orders, etc.)
- Apply all Entity Framework migrations

### 3. Verify Database Creation
After running the migration, you can verify in SQL Server Management Studio:
- Connect to `SSHETEX64VM`
- Check that `CleanMvcAppDb` database exists
- Verify tables like `AspNetUsers`, `Stores`, `Products`, etc. are created

### 4. Start the Application
Once migrations are applied successfully:

```powershell
dotnet run
```

The application will:
- Seed the database with default roles (Admin, StoreOwner, Customer)
- Create a default admin user (admin@localstore.com / Admin@123)
- Start listening on http://localhost:5264

### 5. Test Registration
- Navigate to http://localhost:5264/Account/Register
- Fill in the registration form
- Submit - it should now work without database errors

## Connection String
The application is configured to connect to:
```
Server: SSHETEX64VM
Database: CleanMvcAppDb
Authentication: Windows Integrated Security
```

## Troubleshooting

### If migration fails with "database does not exist":
The migration will automatically create the database if it doesn't exist.

### If you get permission errors:
Make sure your Windows account has permissions to create databases on SSHETEX64VM SQL Server instance.

### If you want to start fresh:
```powershell
dotnet ef database drop --force
dotnet ef database update
```

This will drop and recreate the entire database.
