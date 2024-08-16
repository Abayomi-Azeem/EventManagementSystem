using EventManagementSystem.Api.Common.EventModels;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventManagementSystem.Api.Controllers;

public class AdminController : Controller
{
    private readonly IEventService _eventService;
    private readonly ILogger<AdminController> _logger;
    public AdminController(IEventService eventService, ILogger<AdminController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var events = await _eventService.GetEventResponse(null, null, true);
        return View(events.Value);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        var @event = await _eventService.GetEventResponse(null, id,true);
        if (@event.IsFailure)
        {
            return NotFound();
        }
        var updateReq = new UpdateEventRequest
        {
            Title = @event.Value.SingleEvent.Title,
            Description = @event.Value.SingleEvent.Title,
            Location = @event.Value.SingleEvent.Location,
            Date = @event.Value.SingleEvent.Date,
            MaxParticipants = @event.Value.SingleEvent.MaxParticipants,
            EventId = @event.Value.SingleEvent.Id
        };
        return View(updateReq);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        string userId = TempData["UserId"].ToString();
        var events = await _eventService.GetEventResponse(null, id,true);
        TempData["UserId"] = userId;
        return View(events.Value);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateEventRequest request)
    {
        string userId = TempData["UserId"].ToString();
        var eventId = ViewBag.EventId;
        var eventEdited = await _eventService.UpdateEvent(request, userId, true);
        TempData["UserId"] = userId;
        if (eventEdited.IsFailure)
        {
            return BadRequest(eventEdited.Error);
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        string userId = TempData["UserId"].ToString();
        var @event = await _eventService.GetEventResponse(null, id,true);
        if (@event.IsFailure)
        {
            return NotFound();
        }
        TempData["UserId"] = userId;
        return View(@event.Value);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        string userId = TempData["UserId"].ToString();

        var eventdeleted = await _eventService.DeleteEvent(id, userId,true);
        TempData["UserId"] = userId;

        if (eventdeleted.IsFailure)
        {
            return BadRequest(eventdeleted.Error);
        }
        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> Registrations()
    {
        var response =await _eventService.GetRegistrations();
        return View(response.Value);
    }

}

