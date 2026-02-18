# Apply EF Core Migrations
Write-Host "Applying database migrations..." -ForegroundColor Green

try {
    # Add dotnet tools to PATH
    $env:PATH += ";$env:USERPROFILE\.dotnet\tools"
    
    # Apply migrations
    dotnet ef database update --no-build
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migrations applied successfully!" -ForegroundColor Green
    } else {
        Write-Host "Failed to apply migrations. Exit code: $LASTEXITCODE" -ForegroundColor Red
    }
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
