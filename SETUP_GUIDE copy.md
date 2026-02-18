# Setup Guide

## Quick Start

### 1. Database Setup
Create the initial database migration and update the database:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 2. Run the Application
```bash
dotnet run
```

Access the application at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## Project Configuration

### Installed NuGet Packages
- ✅ Microsoft.EntityFrameworkCore (8.0.23)
- ✅ Microsoft.EntityFrameworkCore.SqlServer (8.0.23)
- ✅ Microsoft.EntityFrameworkCore.Tools (8.0.23)
- ✅ Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.23)
- ✅ Microsoft.AspNetCore.Identity.UI (8.0.23)
- ✅ AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)

### Configured Features
- ✅ Dependency Injection
- ✅ Entity Framework Core with SQL Server
- ✅ ASP.NET Core Identity
- ✅ AutoMapper
- ✅ Structured Logging (Console, Debug, EventSource)
- ✅ Global Error Handling Middleware
- ✅ Razor Views

## Architecture Overview

### Folder Structure
- **Controllers/** - MVC controllers handling HTTP requests
- **Data/** - Database context and EF Core configurations
- **Services/** - Business logic layer with interfaces
- **Mappings/** - AutoMapper profiles for object mapping
- **Middleware/** - Custom middleware components
- **Models/** - View models and domain entities
- **Views/** - Razor view templates

### Key Files
- **Program.cs** - Application startup and service configuration
- **appsettings.json** - Configuration including connection strings
- **ApplicationDbContext.cs** - EF Core database context

## Development Workflow

### Adding a New Feature
1. Create domain models in `Models/`
2. Update `ApplicationDbContext` with DbSet properties
3. Create service interface in `Services/`
4. Implement service with business logic
5. Register service in `Program.cs`
6. Create controller in `Controllers/`
7. Add views in `Views/[ControllerName]/`
8. Create AutoMapper profile if needed

### Database Changes
```bash
# Add a new migration
dotnet ef migrations add [MigrationName]

# Update database
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

## Connection String Options

### LocalDB (Default)
```json
"Server=(localdb)\\mssqllocaldb;Database=CleanMvcAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### SQL Server Express
```json
"Server=localhost\\SQLEXPRESS;Database=CleanMvcAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### SQL Server with Credentials
```json
"Server=your-server;Database=CleanMvcAppDb;User Id=your-user;Password=your-password;MultipleActiveResultSets=true"
```

## Logging Configuration

Logging is configured in `appsettings.json`:
```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "Microsoft.EntityFrameworkCore": "Information"
  }
}
```

## Identity Configuration

Identity is configured in `Program.cs` with:
- Password requirements (6+ characters, uppercase, lowercase, digit)
- No email confirmation required (for development)
- Entity Framework stores

To customize, modify the options in `Program.cs`.

## Error Handling

Global error handling is implemented via `ErrorHandlingMiddleware`:
- Catches all unhandled exceptions
- Logs errors with full stack trace
- Returns JSON error response in production
- Shows developer exception page in development

## Next Steps

1. ✅ Run database migrations
2. Add your domain models
3. Create repository pattern (optional)
4. Implement business services
5. Build controllers and views
6. Add authentication/authorization
7. Configure production settings
8. Add unit tests
