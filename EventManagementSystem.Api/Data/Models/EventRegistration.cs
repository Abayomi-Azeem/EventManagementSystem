namespace EventManagementSystem.Api.Data.Models
{
    public class EventRegistration
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int EventId { get; set; }

        public DateTime DateRegistered { get; set; }

        public Event Event { get; set; }
        public User User { get; set; }
    }
}
