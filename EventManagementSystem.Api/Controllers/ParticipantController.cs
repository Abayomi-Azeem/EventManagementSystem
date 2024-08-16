using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventManagementSystem.Api.Controllers
{
    public class ParticipantController : Controller
	{
		private readonly IEventService _eventService;
		private readonly ILogger<ParticipantController> _logger;

		public ParticipantController(IEventService eventService, ILogger<ParticipantController> logger)
		{
			_eventService = eventService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var events =await _eventService.GetEventResponse(null, null, true);
			return View(events.Value);
		}

        [HttpGet]
		public async Task<IActionResult> RegisterEvent([FromQuery] int id)
		{
			try
			{
				string userId = TempData["UserId"].ToString();
                var registeredRes = await _eventService.RegisterForEvent(id, userId);
				if (registeredRes.Success)
				{
                    ViewBag.IsError = "false";
                    ViewBag.Error = "Registration Successful";
                    return RedirectToAction("Index");
                }
                ViewBag.IsError = "true";
                ViewBag.Error = registeredRes.Error;
                return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				_logger.LogError($"[RegisterEvent] - Exception {JsonConvert.SerializeObject(ex)}");
                ViewBag.IsError = "true";
                ViewBag.Error = "An Error Occurred";
                return View();
            }
		}
				
	}
}
