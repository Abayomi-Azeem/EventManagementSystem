using EventManagementSystem.Api.Common;
using EventManagementSystem.Api.Common.Authentication;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using static EventManagementSystem.Api.Common.AppEnums;

namespace EventManagementSystem.Api.Controllers;


public class AuthController : Controller
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ITokenService tokenService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _tokenService = tokenService;
        _logger = logger;
    }

	public IActionResult Login()
	{
		ViewBag.Title = "Login";
		return View();
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
            {
				ViewBag.IsError = "true";
				ViewBag.Error = authenticatedRes.Error;
				return View();
			}
            var roles = await _userService.GetUserRole(authenticatedRes.Value);
            var token = _tokenService.GenerateAccessToken(authenticatedRes.Value, roles);
			TempData["UserId"] = authenticatedRes.Value.Id;
            if(roles.First() == UserRoles.Participant.ToString())
            {
                return RedirectToAction("Index", "Participant");
            }            
            else if (roles.First() == UserRoles.Organizer.ToString())
            {
                return RedirectToAction("Index", "Events");
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Login] - Exception - {JsonConvert.SerializeObject(ex)}");
			ViewBag.IsError = "true";
			ViewBag.Error = "An Error Occurred";
			return View();
		}
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var userCreatedRes = await _userService.CreateUser(request, UserRoles.Participant);
            if (userCreatedRes.Success)
                return RedirectToAction("Login");
			ViewBag.IsError = "true";
			ViewBag.Error = userCreatedRes.Error;
			return View();
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Register] - Exception - {JsonConvert.SerializeObject(ex)}");
			ViewBag.IsError = "true";
			ViewBag.Error = "An Error Occurred";
			return View();
		}
    }

    public IActionResult Register()
    {
        ViewBag.Title = "Register";
        return View();
    }

    [HttpPost]
    [Route("create-organizer")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> CreateUser(RegisterRequest request)
    {
        try
        {
            var userCreatedRes = await _userService.CreateUser(request, UserRoles.Organizer);
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
