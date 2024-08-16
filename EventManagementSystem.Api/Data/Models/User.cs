using Microsoft.AspNetCore.Identity;

namespace EventManagementSystem.Api.Data.Models
{
    public class User: IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
