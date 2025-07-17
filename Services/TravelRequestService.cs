using Microsoft.EntityFrameworkCore;
using TravelRequestApi.Data;

namespace TravelRequestApi.Services
{
    public class TravelRequestService : ITravelRequestService
    {
        private readonly ApplicationDbContext _context;

        public TravelRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TravelRequest> CreateTravelRequestAsync(CreateTravelRequestDto dto, int userId, string username)
        {
            if (dto.Origin == dto.Destination)
                throw new ArgumentException("La ciudad de origen y destino deben ser diferentes.");

            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("La fecha de regreso debe ser posterior a la fecha de ida.");

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

            return travelRequest;
        }

        public async Task<List<TravelRequest>> GetMyRequestsAsync(int userId)
        {
            return await _context.TravelRequests
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<TravelRequest> UpdateStatusAsync(int id, TravelStatus status)
        {
            var request = await _context.TravelRequests.FindAsync(id);
            if (request == null)
                throw new KeyNotFoundException("Solicitud no encontrada.");

            request.Status = status;
            await _context.SaveChangesAsync();

            return request;
        }
    }
}