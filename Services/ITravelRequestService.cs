using TravelRequestApi.Data;

namespace TravelRequestApi.Services
{
    public interface ITravelRequestService
    {
        Task<TravelRequest> CreateTravelRequestAsync(CreateTravelRequestDto dto, int userId, string username);
        Task<List<TravelRequest>> GetMyRequestsAsync(int userId);
        Task<TravelRequest> UpdateStatusAsync(int id, TravelStatus status);
    }
}