using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Api.Common.EventModels;

public class CreateEventRequest
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Location { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public int MaxParticipants  { get; set; }
}
