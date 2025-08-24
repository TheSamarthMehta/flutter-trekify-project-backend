# SQL Server Migration Guide

## Overview
The Trekify backend has been converted from MongoDB to Microsoft SQL Server using Entity Framework Core.

## Prerequisites

### For Local Development:
1. **SQL Server LocalDB** (included with Visual Studio) OR
2. **SQL Server Express** (free version) OR
3. **SQL Server Developer Edition** (free for development)

### For Production:
1. **Azure SQL Database** (recommended for cloud deployment)
2. **SQL Server on Azure VM**
3. **On-premises SQL Server**

## Local Setup

### Option 1: SQL Server LocalDB (Recommended for Development)
```bash
# No installation needed if you have Visual Studio
# Connection string will be:
# Server=(localdb)\\mssqllocaldb;Database=TrekifyDB;Trusted_Connection=true;
```

### Option 2: SQL Server Express
1. Download SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads
2. Install with default settings
3. Use connection string: `Server=localhost\\SQLEXPRESS;Database=TrekifyDB;Trusted_Connection=true;TrustServerCertificate=true;`

### Option 3: Docker SQL Server
```bash
# Run SQL Server in Docker
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sql-server -d mcr.microsoft.com/mssql/server:2022-latest

# Connection string:
# Server=localhost,1433;Database=TrekifyDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
```

## Environment Variables

Update your `.env` file:

```env
# SQL Server Configuration
SQLSERVER_CONNECTION_STRING=Server=localhost;Database=TrekifyDB;Trusted_Connection=true;TrustServerCertificate=true;
DATABASE_NAME=TrekifyDB

# For Docker SQL Server:
# SQLSERVER_CONNECTION_STRING=Server=localhost,1433;Database=TrekifyDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;

# For Azure SQL Database:
# SQLSERVER_CONNECTION_STRING=Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=TrekifyDB;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## Database Schema

The application will automatically create the following tables:

### Users Table
```sql
CREATE TABLE Users (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Email nvarchar(255) NOT NULL UNIQUE,
    Password nvarchar(255) NOT NULL,
    Avatar nvarchar(500) NULL,
    Date datetime2 DEFAULT GETUTCDATE()
);
```

### Treks Table
```sql
CREATE TABLE Treks (
    Id int IDENTITY(1,1) PRIMARY KEY,
    SerialNumber int NOT NULL,
    State nvarchar(100),
    TrekName nvarchar(200),
    TrekType nvarchar(100),
    DifficultyLevel nvarchar(50),
    Season nvarchar(100),
    Duration nvarchar(100),
    Distance nvarchar(100),
    MaxAltitude nvarchar(100),
    TrekDescription nvarchar(MAX),
    Image nvarchar(500)
);
```

## Data Migration

The application will:
1. **Auto-create database** on first run
2. **Load Excel data** automatically when `/api/data` is first accessed
3. **Store data in SQL Server** for subsequent requests

## Production Deployment

### Azure SQL Database (Recommended)

1. **Create Azure SQL Database**:
   ```bash
   # Using Azure CLI
   az sql server create --name your-server-name --resource-group your-rg --location "East US" --admin-user youradmin --admin-password YourStrong@Password
   az sql db create --resource-group your-rg --server your-server-name --name TrekifyDB --service-objective Basic
   ```

2. **Get Connection String**:
   - Go to Azure Portal → SQL Database → Connection Strings
   - Copy the ADO.NET connection string

3. **Update Render Environment Variables**:
   ```
   SQLSERVER_CONNECTION_STRING=Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=TrekifyDB;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```

4. **Configure Firewall**:
   - Add your IP address to Azure SQL firewall rules
   - Add Azure services to allowed list

## Running the Application

```bash
# Navigate to project directory
cd TrekifyBackend

# Restore packages
dotnet restore

# Run the application
dotnet run
```

The application will:
1. Connect to SQL Server
2. Create database if it doesn't exist
3. Create tables using Entity Framework
4. Load trek data from Excel file on first API call

## Troubleshooting

### Connection Issues
```bash
# Test SQL Server connection
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# Check if SQL Server is running
Get-Service MSSQLSERVER
```

### Common Connection Strings

**Local Development (Windows Authentication)**:
```
Server=localhost;Database=TrekifyDB;Trusted_Connection=true;TrustServerCertificate=true;
```

**Local Development (SQL Authentication)**:
```
Server=localhost;Database=TrekifyDB;User Id=sa;Password=YourPassword;TrustServerCertificate=true;
```

**Docker SQL Server**:
```
Server=localhost,1433;Database=TrekifyDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;
```

**Azure SQL Database**:
```
Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=TrekifyDB;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## Migration from MongoDB

If you have existing MongoDB data, you can:
1. Export data from MongoDB to JSON
2. Create a data seeder script to import to SQL Server
3. Or manually recreate user accounts (trek data loads from Excel automatically)

## Benefits of SQL Server

1. **ACID Compliance**: Better data consistency
2. **SQL Queries**: Standard SQL support
3. **Scalability**: Better performance for complex queries
4. **Integration**: Better integration with .NET ecosystem
5. **Tooling**: Rich management tools (SQL Server Management Studio)
6. **Cloud**: Seamless Azure integration
