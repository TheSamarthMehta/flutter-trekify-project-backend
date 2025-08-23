# Render Build Script
echo "Starting Render build process..."

# Restore dependencies
echo "Restoring NuGet packages..."
dotnet restore

# Build the application
echo "Building the application..."
dotnet build --configuration Release --no-restore

# Publish the application
echo "Publishing the application..."
dotnet publish --configuration Release --no-build --output ./publish

echo "Build completed successfully!"
