# Use the official .NET 8.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 8.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TrekifyBackend/TrekifyBackend.csproj", "TrekifyBackend/"]
RUN dotnet restore "TrekifyBackend/TrekifyBackend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TrekifyBackend/TrekifyBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrekifyBackend/TrekifyBackend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrekifyBackend.dll"]
