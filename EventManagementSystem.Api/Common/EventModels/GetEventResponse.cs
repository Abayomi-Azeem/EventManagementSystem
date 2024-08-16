using EventManagementSystem.Api.Data.Models;

namespace EventManagementSystem.Api.Common.EventModels
{
    public class GetEventResponse
    {
        public List<Event>? Events { get; set; }

        public Event? SingleEvent { get; set; }
    }

    
}
