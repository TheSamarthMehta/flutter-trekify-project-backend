using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TrekifyBackend.Models;
using BCrypt.Net;

namespace TrekifyBackend.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET") 
                        ?? _configuration["JwtSettings:Secret"] 
                        ?? "this_is_a_very_long_secret_key_for_development_only_32_chars_minimum";
            
            var expiryInDays = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_DAYS"), out int envDays) 
                              ? envDays 
                              : int.TryParse(_configuration["JwtSettings:ExpiryInDays"], out int configDays) 
                                ? configDays 
                                : 5;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(expiryInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 10);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
