using EventManagementSystem.Api.Common;
using EventManagementSystem.Api.Common.Authentication;
using EventManagementSystem.Api.Data.Models;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using static EventManagementSystem.Api.Common.AppEnums;

namespace EventManagementSystem.Api.Services.Implementations;

public class UserService: IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<UserService> _userService;
    private readonly SignInManager<User> _signInManager;

    public UserService(ILogger<UserService> userService, RoleManager<Role> roleManager, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userService = userService;
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<Result<User>> AuthenticateUser(LoginRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is null)
            return Result.Fail<User>("Email or Password Incorrect");

        var passwordCheck = _userManager.PasswordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, request.Password);
        if (passwordCheck == PasswordVerificationResult.Failed)
            return Result.Fail<User>("Email or Password Incorrect");
        await _signInManager.PasswordSignInAsync(existingUser.Email, request.Password, false, false);
        return Result.Ok<User>(existingUser);
    }

    public async Task<List<string>> GetUserRole(User user)
    {
        var role =await _userManager.GetRolesAsync(user);
        return role.ToList();
    }

    public async Task<Result<string>> CreateUser(RegisterRequest request, UserRoles role)
    {
        var checkExist = await _userManager.FindByEmailAsync(request.Email);
        if (checkExist is not null)
            return Result.Fail<string>("User already exists");

        User user = new User()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            EmailConfirmed = true,
            UserName = request.Email,
            NormalizedUserName = request.Email.ToUpper(),
        };

        var userCreationRes = await _userManager.CreateAsync(user, request.Password);
        if (!userCreationRes.Succeeded)
            return Result.Fail<string>(userCreationRes.Errors.FirstOrDefault()!.Description ?? "User Creation Failed");

        var assignRole = await _userManager.AddToRoleAsync(user,role.ToString());
        if (!assignRole.Succeeded)
            return Result.Fail<string>("Error Assigning UserRole");

        return Result.Ok<string>("User Created Successfully");
    }
}
