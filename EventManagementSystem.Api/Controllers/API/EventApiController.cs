using Azure.Core;
using EventManagementSystem.Api.Common.EventModels;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace EventManagementSystem.Api.Controllers.API;

[Route("api/event")]
[ApiController]
public class EventApiController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ILogger<EventApiController> _logger;
    private readonly IEmailService _emailService;

    public EventApiController(IEventService eventService, ILogger<EventApiController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "OrganizerPolicy")]
    public async Task<IActionResult> CreateEvent(CreateEventRequest request)
    {
        try
        {
            var organizerId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
            if (string.IsNullOrEmpty(organizerId))
                return Unauthorized();
            var createdEventRes = await _eventService.CreateEvent(request, organizerId);
            if (createdEventRes.Success)
                return Ok(createdEventRes.Value);
            return Problem(statusCode: 400, title: createdEventRes.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[CreateEvent] - Exception {JsonConvert.SerializeObject(ex)}");
            return Problem(statusCode: 500, title: "An Error Occurred");
        }
    }

    [HttpPut]
    [Authorize(Policy = "OrganizerPolicy")]
    public async Task<IActionResult> EditEvent(UpdateEventRequest request)
    {
        try
        {
            var organizerId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
            if (string.IsNullOrEmpty(organizerId))
                return Unauthorized();
            var editedEventRes = await _eventService.UpdateEvent(request, organizerId);
            if (editedEventRes.Success)
                return Ok(editedEventRes.Value);
            return Problem(statusCode: 400, title: editedEventRes.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[EditEvent] - Exception {JsonConvert.SerializeObject(ex)}");
            return Problem(statusCode: 500, title: "An Error Occurred");
        }
    }

    //delete event
    [HttpDelete("{id}")]
    [Authorize(Policy = "OrganizerPolicy")]
    public async Task<IActionResult> DeleteEvent([FromRoute] int id)
    {
        try
        {
            var organizerId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
            if (string.IsNullOrEmpty(organizerId))
                return Unauthorized();
            var delEventRes = await _eventService.DeleteEvent(id, organizerId);
            if (delEventRes.Success)
                return Ok(delEventRes.Value);
            return Problem(statusCode: 400, title: delEventRes.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[DeleteEvent] - Exception {JsonConvert.SerializeObject(ex)}");
            return Problem(statusCode: 500, title: "An Error Occurred");
        }
    }



    [HttpGet]
    [Authorize(Policy = "OrganizerPolicy")]
    public async Task<IActionResult> GetEventsForUser()
    {
        try
        {
            var participantId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
            if (string.IsNullOrEmpty(participantId))
                return Unauthorized();
            var events = await _eventService.GetEventResponse(participantId, null);
            if (events.Success)
                return Ok(events.Value);
            return Problem(statusCode: 400, title: events.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[GetEventsForUser] - Exception {JsonConvert.SerializeObject(ex)}");
            return Problem(statusCode: 500, title: "An Error Occurred");
            ;
        }
    }

    [HttpGet("id")]
    [Authorize(Policy = "OrganizerPolicy")]
    public async Task<IActionResult> GetEventsForUser([FromRoute] int id)
    {
        try
        {
            var participantId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
            if (string.IsNullOrEmpty(participantId))
                return Unauthorized();
            var events = await _eventService.GetEventResponse(participantId, id);
            if (events.Success)
                return Ok(events.Value);
            return Problem(statusCode: 400, title: events.Error);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[GetEventsForUser] - Exception {JsonConvert.SerializeObject(ex)}");
            return Problem(statusCode: 500, title: "An Error Occurred");
        }
    }


}
