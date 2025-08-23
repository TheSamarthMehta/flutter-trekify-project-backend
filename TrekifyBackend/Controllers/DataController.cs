using Microsoft.AspNetCore.Mvc;
using TrekifyBackend.Services;

namespace TrekifyBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;

        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// Get all trek data
        /// </summary>
        /// <returns>List of all treks</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTreks()
        {
            try
            {
                var treks = await _dataService.GetAllTreksAsync();
                return Ok(treks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get treks by state name
        /// </summary>
        /// <param name="stateName">Name of the state</param>
        /// <returns>List of treks in the specified state</returns>
        [HttpGet("state/{stateName}")]
        public async Task<IActionResult> GetTreksByState(string stateName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stateName))
                {
                    return BadRequest(new { message = "State name is required" });
                }

                var decodedStateName = Uri.UnescapeDataString(stateName);
                var treks = await _dataService.GetTreksByStateAsync(decodedStateName);

                if (!treks.Any())
                {
                    return NotFound(new { message = "No data found for this state." });
                }

                return Ok(treks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get trek by serial number (ID)
        /// </summary>
        /// <param name="id">Serial number of the trek</param>
        /// <returns>Trek details</returns>
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetTrekById(int id)
        {
            try
            {
                var trek = await _dataService.GetTrekByIdAsync(id);

                if (trek == null)
                {
                    return NotFound(new { message = "Trek not found." });
                }

                return Ok(trek);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { msg = "Server error", error = ex.Message });
            }
        }
    }
}
