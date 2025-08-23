using Microsoft.AspNetCore.Mvc;

namespace TrekifyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OnboardingController : ControllerBase
    {
        /// <summary>
        /// Get the URLs for onboarding media
        /// </summary>
        /// <returns>List of onboarding data</returns>
        [HttpGet]
        public IActionResult GetOnboardingData()
        {
            try
            {
                var onboardingData = new[]
                {
                    new
                    {
                        path = "https://res.cloudinary.com/dvnr3ouix/video/upload/v1754843270/welcome_s2tezu.mp4",
                        isVideo = true,
                        title = "Welcome to Trekify!",
                        description = "Your personal guide to the world of trekking. Let's find the perfect adventure for you."
                    },
                    new
                    {
                        path = "https://res.cloudinary.com/dvnr3ouix/image/upload/v1754843277/difficulty_g4hwbk.jpg",
                        isVideo = false,
                        title = "Discover Your Path",
                        description = "Explore treks of all types, from serene lakes to challenging mountain forts."
                    },
                    new
                    {
                        path = "https://res.cloudinary.com/dvnr3ouix/image/upload/v1754843275/type_urd4zs.jpg",
                        isVideo = false,
                        title = "Track Your Journey",
                        description = "Save your favorite treks to a wishlist and mark the ones you've conquered."
                    }
                };

                return Ok(onboardingData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Server error", error = ex.Message });
            }
        }
    }
}
