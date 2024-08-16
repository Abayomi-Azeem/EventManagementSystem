using EventManagementSystem.Api.Common;
using EventManagementSystem.Api.Common.EventModels;

namespace EventManagementSystem.Api.Services.Interfaces
{
    public interface IEventService
    {
        Task<Result<string>> CreateEvent(CreateEventRequest request, string organizerId);
        Task<Result<string>> DeleteEvent(int eventId, string organizerId, bool isAdmin=false);
        Task<Result<string>> UpdateEvent(UpdateEventRequest request, string organizerId, bool isAdmin=false);
        Task<Result<string>> RegisterForEvent(int eventId, string userId);
        Task<Result<GetEventResponse>> GetEventResponse(string? userId, int? eventId, bool isAdminOrUser= false);
        Task<Result<List<GetRegistrationsResponse>>> GetRegistrations();
    }
}
