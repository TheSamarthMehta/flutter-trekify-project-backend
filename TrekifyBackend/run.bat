@echo off
echo Starting Trekify .NET Backend...
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK is not installed or not in PATH.
    echo Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo .NET SDK Version:
dotnet --version
echo.

REM Navigate to the project directory
cd /d "%~dp0"

REM Restore packages
echo Restoring NuGet packages...
dotnet restore
if errorlevel 1 (
    echo ERROR: Failed to restore packages.
    pause
    exit /b 1
)

REM Run the application
echo.
echo Starting the application...
echo API will be available at: http://localhost:5000
echo Swagger UI will be available at: http://localhost:5000/swagger
echo.
dotnet run

pause
