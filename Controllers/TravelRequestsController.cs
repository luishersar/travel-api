using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelRequestApi.Data;


namespace DemoTravelApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TravelRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TravelRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/TravelRequests
        [HttpPost]
        public async Task<IActionResult> CreateTravelRequest([FromBody] CreateTravelRequestDto dto)
        {
            if (dto.Origin == dto.Destination)
                return BadRequest("La ciudad de origen y destino deben ser diferentes.");

            if (dto.StartDate >= dto.EndDate)
                return BadRequest("La fecha de regreso debe ser posterior a la fecha de ida.");

            var username = User.FindFirstValue(ClaimTypes.Name);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);


            var travelRequest = new TravelRequest
            {
                EmployeeName = username ?? "Unknown",
                Origin = dto.Origin,
                Destination = dto.Destination,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Purpose = dto.Purpose,
                Status = TravelStatus.Pending,
                UserId = userId 

            };

            _context.TravelRequests.Add(travelRequest);
            await _context.SaveChangesAsync();

            return Ok(travelRequest);
        }

        // GET: api/TravelRequests/mine
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyRequests()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var requests = await _context.TravelRequests
                .Where(r => r.EmployeeName == username)
                .ToListAsync();

            return Ok(requests);
        }

        // PUT: api/TravelRequests/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Aprobador")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] TravelStatus status)
        {
            var request = await _context.TravelRequests.FindAsync(id);
            if (request == null)
                return NotFound("Solicitud no encontrada.");

            request.Status = status;
            await _context.SaveChangesAsync();

            return Ok(request);
        }
    }
}
