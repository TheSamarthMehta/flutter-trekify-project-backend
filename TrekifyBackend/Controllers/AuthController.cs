using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TrekifyBackend.Models;
using TrekifyBackend.Services;

namespace TrekifyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if user already exists
                var existingUser = await _userService.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { msg = "User already exists" });
                }

                // Create new user
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = _authService.HashPassword(request.Password)
                };

                await _userService.CreateAsync(user);

                // Generate JWT token
                var token = _authService.GenerateJwtToken(user);

                return Ok(new AuthResponse { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Server error", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if user exists
                var user = await _userService.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    return NotFound(new { msg = "User not found. Please sign up." });
                }

                // Verify password
                if (!_authService.VerifyPassword(request.Password, user.Password))
                {
                    return BadRequest(new { msg = "Invalid credentials" });
                }

                // Generate JWT token
                var token = _authService.GenerateJwtToken(user);

                return Ok(new AuthResponse { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Server error", error = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = HttpContext.Items["UserId"]?.ToString();
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { msg = "No token, authorization denied" });
                }

                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { msg = "User not found" });
                }

                // Return user without password
                var userResponse = new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    avatar = user.Avatar,
                    date = user.Date
                };

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Server error", error = ex.Message });
            }
        }
    }
}
