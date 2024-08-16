using EventManagementSystem.Api.Common.Authentication;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using static EventManagementSystem.Api.Common.AppEnums;

namespace EventManagementSystem.Api.Controllers.API;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthApiController> _logger;

    public AuthApiController(IUserService userService, ITokenService tokenService, ILogger<AuthApiController> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }


    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var authenticatedRes = await _userService.AuthenticateUser(request);
            if (authenticatedRes.IsFailure)
                return Problem(statusCode: 400, title: authenticatedRes.Error);
            var roles = await _userService.GetUserRole(authenticatedRes.Value);
            var token = _tokenService.GenerateAccessToken(authenticatedRes.Value, roles);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Login] - Exception - {JsonConvert.SerializeObject(ex)}");
            return Problem(statusCode: 500, title: "An Error Occurred");
        }
    }

    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var userCreatedRes = await _userService.CreateUser(request, Common.AppEnums.UserRoles.Participant);
            if (userCreatedRes.Success)
                return Ok(userCreatedRes.Value);
            return Problem(statusCode: 400, title: userCreatedRes.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Register] - Exception - {JsonConvert.SerializeObject(ex)}");
            return Problem(statusCode: 500, title: "An Error Occurred");
        }
    }

    [HttpPost]
    [Route("create-organizer")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> CreateUser(RegisterRequest request)
    {
        try
        {
            var userCreatedRes = await _userService.CreateUser(request, Common.AppEnums.UserRoles.Organizer);
            if (userCreatedRes.Success)
                return Ok(userCreatedRes.Value);
            return Problem(statusCode: 400, title: userCreatedRes.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Register] - Exception - {JsonConvert.SerializeObject(ex)}");
            return Problem(statusCode: 500, title: "An Error Occurred");
        }
    }


}
