using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TrekifyBackend.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["x-auth-token"].FirstOrDefault();

            if (token != null)
                AttachUserToContext(context, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secret = Environment.GetEnvironmentVariable("JWT_SECRET") 
                            ?? _configuration["JwtSettings:Secret"] 
                            ?? "this_is_a_very_long_secret_key_for_development_only_32_chars_minimum";
                var key = Encoding.UTF8.GetBytes(secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userIdString = jwtToken.Claims.First(x => x.Type == "id").Value;
                
                if (int.TryParse(userIdString, out int userId))
                {
                    // Attach user to context
                    context.Items["UserId"] = userId;
                }
                else
                {
                    // Handle legacy string IDs if needed
                    context.Items["UserId"] = userIdString;
                }
            }
            catch
            {
                // Do nothing if JWT validation fails
                // User is not attached to context so the request won't have access to secure routes
            }
        }
    }

    // Attribute to secure endpoints
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {
        public void OnAuthorization(Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Items["UserId"];
            if (userId == null)
            {
                // Not logged in
                context.Result = new Microsoft.AspNetCore.Mvc.JsonResult(new { message = "Unauthorized" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}
