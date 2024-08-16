namespace EventManagementSystem.Api.Common.EventModels;

public class UpdateEventRequest: CreateEventRequest
{
    public int EventId { get; set; }
}
