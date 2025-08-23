# Trekify Backend - .NET Core Web API

This is a .NET Core Web API version of the Trekify backend, converted from the original Node.js version.

## Features

- User authentication with JWT tokens
- User registration and login
- Trek data management from Excel files
- State-based trek filtering
- CORS enabled for cross-origin requests
- MongoDB integration
- Swagger API documentation

## Prerequisites

- .NET 8.0 SDK
- MongoDB database
- Visual Studio 2022 or VS Code

## Setup Instructions

1. **Install .NET 8.0 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0

2. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd TrekifyBackend
   ```

3. **Install dependencies**
   ```bash
   dotnet restore
   ```

4. **Configure the application**
   - Update `appsettings.json` with your MongoDB connection string
   - Set your JWT secret key (should be at least 32 characters)

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the API**
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login user
- `GET /api/auth/me` - Get current user (requires token)

### Data
- `GET /api/data` - Get all treks
- `GET /api/data/state/{stateName}` - Get treks by state
- `GET /api/data/id/{id}` - Get trek by ID

### Onboarding
- `GET /api/onboarding` - Get onboarding media URLs

## Project Structure

```
TrekifyBackend/
├── Controllers/          # API Controllers
├── Models/              # Data models
├── Services/            # Business logic services
├── Middleware/          # Custom middleware
├── Data/               # Data access layer
├── Program.cs          # Application entry point
├── appsettings.json    # Configuration
└── TrekifyBackend.csproj # Project file
```

## Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "MongoDB": "your_mongodb_connection_string"
  },
  "DatabaseSettings": {
    "DatabaseName": "trekify"
  },
  "JwtSettings": {
    "Secret": "your_jwt_secret_key_here",
    "ExpiryInDays": 5
  }
}
```

## Dependencies

- **MongoDB.Driver** - MongoDB connectivity
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication
- **BCrypt.Net-Next** - Password hashing
- **EPPlus** - Excel file processing
- **Swashbuckle.AspNetCore** - Swagger documentation

## Migration from Node.js

This .NET version maintains API compatibility with the original Node.js backend:

- Same endpoint URLs and request/response formats
- Equivalent authentication mechanism
- Same data processing logic
- Compatible with existing Flutter frontend

## Development

1. **Run in development mode**
   ```bash
   dotnet run --environment Development
   ```

2. **Build for production**
   ```bash
   dotnet publish -c Release
   ```

3. **Run tests** (when tests are added)
   ```bash
   dotnet test
   ```

## Deployment

The application can be deployed to:
- Azure App Service
- AWS Elastic Beanstalk
- Docker containers
- IIS
- Linux servers with .NET runtime

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request
