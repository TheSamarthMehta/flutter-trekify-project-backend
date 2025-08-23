# Stage 1: Define the base image for the final application
# This uses the ASP.NET Core runtime, which is smaller than the SDK image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Stage 2: Build the application
# This uses the full .NET SDK to build the project.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file first and restore dependencies.
# This leverages Docker's layer caching. If the project file doesn't change,
# the dependencies won't be re-downloaded on subsequent builds.
COPY ["TrekifyBackend/TrekifyBackend.csproj", "TrekifyBackend/"]
RUN dotnet restore "TrekifyBackend/TrekifyBackend.csproj"

# Copy the rest of the application source code
COPY . .

# Build the project. The output will be placed in the specified directory.
WORKDIR "/src/TrekifyBackend"
RUN dotnet build "TrekifyBackend.csproj" -c Release -o /app/build

# Stage 3: Publish the application
# This stage takes the built output and prepares it for deployment.
FROM build AS publish
RUN dotnet publish "TrekifyBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Create the final, small runtime image
FROM base AS final
WORKDIR /app

# Copy the published output from the 'publish' stage into the final image.
COPY --from=publish /app/publish .

# Copy the data folder from the build context
COPY data/ ./data/

# Set the entry point for the container to run the application.
ENTRYPOINT ["dotnet", "TrekifyBackend.dll"]
