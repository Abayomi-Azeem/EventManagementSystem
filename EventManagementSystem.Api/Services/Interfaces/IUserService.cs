using EventManagementSystem.Api.Common;
using EventManagementSystem.Api.Common.Authentication;
using EventManagementSystem.Api.Data.Models;
using static EventManagementSystem.Api.Common.AppEnums;

namespace EventManagementSystem.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> AuthenticateUser(LoginRequest request);
        Task<List<string>> GetUserRole(User user);
        Task<Result<string>> CreateUser(RegisterRequest request, UserRoles role);
    }
}
