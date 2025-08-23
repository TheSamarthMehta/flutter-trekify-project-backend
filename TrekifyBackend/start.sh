# Render Start Script
echo "Starting Trekify .NET Backend..."

# Navigate to publish directory
cd publish

# Set the port from Render's environment variable
export ASPNETCORE_URLS="http://0.0.0.0:$PORT"

# Start the application
echo "Starting application on port $PORT"
dotnet TrekifyBackend.dll
