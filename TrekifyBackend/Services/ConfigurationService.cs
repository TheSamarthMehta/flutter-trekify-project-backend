namespace TrekifyBackend.Services
{
    public interface IConfigurationService
    {
        string GetMongoConnectionString();
        string GetDatabaseName();
        string GetJwtSecret();
        int GetJwtExpiryDays();
        string[] GetAllowedOrigins();
        string GetExcelDataPath();
        string GetPort();
        string GetAspNetCoreUrls();
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;

        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetMongoConnectionString()
        {
            return Environment.GetEnvironmentVariable("MONGODB_URI")
                   ?? _configuration.GetConnectionString("MongoDB")
                   ?? "mongodb://localhost:27017";
        }

        public string GetDatabaseName()
        {
            return Environment.GetEnvironmentVariable("DATABASE_NAME")
                   ?? _configuration["DatabaseSettings:DatabaseName"]
                   ?? "trekify";
        }

        public string GetJwtSecret()
        {
            return Environment.GetEnvironmentVariable("JWT_SECRET")
                   ?? _configuration["JwtSettings:Secret"]
                   ?? "this_is_a_very_long_secret_key_for_development_only_32_chars_minimum";
        }

        public int GetJwtExpiryDays()
        {
            return int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_DAYS"), out int envDays)
                   ? envDays
                   : int.TryParse(_configuration["JwtSettings:ExpiryInDays"], out int configDays)
                     ? configDays
                     : 5;
        }

        public string[] GetAllowedOrigins()
        {
            var origins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
            if (!string.IsNullOrEmpty(origins))
            {
                return origins.Split(',', StringSplitOptions.RemoveEmptyEntries);
            }
            
            return new[] { "http://localhost:3000", "http://localhost:8080", "http://localhost:4200" };
        }

        public string GetExcelDataPath()
        {
            return Environment.GetEnvironmentVariable("EXCEL_DATA_PATH")
                   ?? "../data/Flutter Data Set.xlsx";
        }

        public string GetPort()
        {
            return Environment.GetEnvironmentVariable("PORT") ?? "5000";
        }

        public string GetAspNetCoreUrls()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
                   ?? $"http://0.0.0.0:{GetPort()}";
        }
    }
}
