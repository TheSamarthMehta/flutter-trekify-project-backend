using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TrekifyBackend.Services;
using TrekifyBackend.Middleware;
using TrekifyBackend.Data;
using DotNetEnv;

// Load environment variables from .env file
var envFile = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envFile))
{
    Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQL Server Database
var connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING") 
                      ?? builder.Configuration.GetConnectionString("DefaultConnection") 
                      ?? "Server=localhost;Database=TrekifyDB;Trusted_Connection=true;TrustServerCertificate=true;";

builder.Services.AddDbContext<TrekifyDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure JWT Authentication
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") 
               ?? builder.Configuration["JwtSettings:Secret"] 
               ?? "this_is_a_very_long_secret_key_for_development_only_32_chars_minimum";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IUserService, UserService>();

// Configure CORS
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') 
                    ?? new[] { "http://localhost:3000", "http://localhost:8080", "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    
    // Fallback policy for development
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TrekifyDbContext>();
    context.Database.EnsureCreated();
    Console.WriteLine("Database ensured created");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

// Custom middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

// Base route
app.MapGet("/", () => "Trekify API (.NET) is up and running...");

// Health check endpoint
app.MapGet("/health", () => new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
});

// Configure for Render deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? $"http://0.0.0.0:{port}";

app.Run(urls);
