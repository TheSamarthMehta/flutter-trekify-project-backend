# Trekify .NET Backend Startup Script

Write-Host "Starting Trekify .NET Backend..." -ForegroundColor Green
Write-Host ""

# Check if .NET is installed
try {
    $dotnetVersion = dotnet --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host ".NET SDK Version: $dotnetVersion" -ForegroundColor Cyan
    } else {
        throw "dotnet command not found"
    }
} catch {
    Write-Host "ERROR: .NET SDK is not installed or not in PATH." -ForegroundColor Red
    Write-Host "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

# Navigate to script directory
Set-Location $PSScriptRoot

# Restore packages
Write-Host ""
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
try {
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        throw "Package restore failed"
    }
} catch {
    Write-Host "ERROR: Failed to restore packages." -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# Run the application
Write-Host ""
Write-Host "Starting the application..." -ForegroundColor Green
Write-Host "API will be available at: http://localhost:5000" -ForegroundColor Cyan
Write-Host "Swagger UI will be available at: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

dotnet run
