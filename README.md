# Clean ASP.NET MVC Application

A scalable ASP.NET Core 8.0 MVC application with clean architecture principles.

## Features

- ✅ ASP.NET Core 8.0 (LTS)
- ✅ Razor Views enabled
- ✅ Entity Framework Core 8.0
- ✅ SQL Server support
- ✅ ASP.NET Core Identity for authentication
- ✅ AutoMapper for object mapping
- ✅ Dependency Injection configured
- ✅ Structured logging
- ✅ Global error handling middleware

## Project Structure

```
CleanMvcApp/
├── Controllers/         # MVC Controllers
├── Data/               # Database context and configurations
├── Mappings/           # AutoMapper profiles
├── Middleware/         # Custom middleware components
├── Models/             # View models and domain models
├── Services/           # Business logic and services
├── Views/              # Razor views
└── wwwroot/            # Static files (CSS, JS, images)
```

## Configuration

### Connection String
Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CleanMvcAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### Database Migration
Run the following commands to create the database:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Running the Application

```bash
dotnet run
```

The application will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## NuGet Packages

- Microsoft.EntityFrameworkCore (8.0.*)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.*)
- Microsoft.EntityFrameworkCore.Tools (8.0.*)
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.*)
- AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)

## Architecture Highlights

### Dependency Injection
Services are registered in `Program.cs` and injected via constructor injection.

### Error Handling
Global error handling is implemented via `ErrorHandlingMiddleware` for consistent error responses.

### Logging
Configured with multiple providers (Console, Debug, EventSource) for comprehensive logging.

### AutoMapper
Mapping profiles are defined in the `Mappings` folder and automatically discovered.

## Next Steps

1. Run database migrations
2. Add your domain models
3. Create repositories for data access
4. Implement business logic in services
5. Build controllers and views
6. Configure authentication pages
