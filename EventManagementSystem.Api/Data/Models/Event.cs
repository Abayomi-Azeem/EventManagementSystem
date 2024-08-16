namespace EventManagementSystem.Api.Data.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public int MaxParticipants { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Deleted { get; set; } = false;

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
