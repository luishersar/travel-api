using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelRequestApi.Services;

namespace TravelRequestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TravelRequestsController : ControllerBase
    {
        private readonly ITravelRequestService _travelRequestService;

        public TravelRequestsController(ITravelRequestService travelRequestService)
        {
            _travelRequestService = travelRequestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTravelRequest([FromBody] CreateTravelRequestDto dto)
        {
            try
            {
                var username = User.FindFirstValue(ClaimTypes.Name);
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userIdClaim == null) return Unauthorized();

                int userId = int.Parse(userIdClaim);

                var result = await _travelRequestService.CreateTravelRequestAsync(dto, userId, username);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyRequests()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null) return Unauthorized();

                int userId = int.Parse(userIdClaim);

                var requests = await _travelRequestService.GetMyRequestsAsync(userId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Aprobador")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] TravelStatus status)
        {
            try
            {
                var result = await _travelRequestService.UpdateStatusAsync(id, status);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}