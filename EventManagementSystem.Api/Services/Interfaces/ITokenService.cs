using EventManagementSystem.Api.Data.Models;

namespace EventManagementSystem.Api.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user, List<string> role);
    }
}
