using EventManagementSystem.Api.Common.EventModels;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EventManagementSystem.Api.Controllers.API
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    public class AdminApiController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<AdminApiController> _logger;
        public AdminApiController(IEventService eventService, ILogger<AdminApiController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        //get all events
        [HttpGet]
        [Route("events/all")]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var adminId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();
                var events = await _eventService.GetEventResponse(null, null, true);
                if (events.Success)
                    return Ok(events.Value);
                return Problem(statusCode: 400, title: events.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetAllEvents] - Exception {JsonConvert.SerializeObject(ex)}");
                return Problem(statusCode: 500, title: "An Error Occurred");
                ;
            }
        }
        //get all events for user

        [HttpGet]
        [Route("events/{userId}")]
        public async Task<IActionResult> GetAllEventsForUser([FromRoute] int userId)
        {
            try
            {
                var adminId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();
                var events = await _eventService.GetEventResponse(userId.ToString(), null, true);
                if (events.Success)
                    return Ok(events.Value);
                return Problem(statusCode: 400, title: events.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetAllEventsForUser] - Exception {JsonConvert.SerializeObject(ex)}");
                return Problem(statusCode: 500, title: "An Error Occurred");
                ;
            }
        }

        [HttpGet]
        [Route("event/{eventId}")]
        public async Task<IActionResult> GetEvent([FromRoute] int eventId)
        {
            try
            {
                var adminId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();
                var events = await _eventService.GetEventResponse(null, eventId, true);
                if (events.Success)
                    return Ok(events.Value);
                return Problem(statusCode: 400, title: events.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GetEvent] - Exception {JsonConvert.SerializeObject(ex)}");
                return Problem(statusCode: 500, title: "An Error Occurred");
                ;
            }
        }

        [HttpPut]
        [Route("event")]
        public async Task<IActionResult> EditEvent(UpdateEventRequest request)
        {
            try
            {
                var adminId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();
                var events = await _eventService.UpdateEvent(request, adminId, true);
                if (events.Success)
                    return Ok(events.Value);
                return Problem(statusCode: 400, title: events.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EditEvent] - Exception {JsonConvert.SerializeObject(ex)}");
                return Problem(statusCode: 500, title: "An Error Occurred");
                ;
            }
        }

        [HttpDelete]
        [Route("event/{eventId}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int eventId)
        {
            try
            {
                var adminId = User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value;
                if (string.IsNullOrEmpty(adminId))
                    return Unauthorized();
                var events = await _eventService.DeleteEvent(eventId, adminId, true);
                if (events.Success)
                    return Ok(events.Value);
                return Problem(statusCode: 400, title: events.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[EditEvent] - Exception {JsonConvert.SerializeObject(ex)}");
                return Problem(statusCode: 500, title: "An Error Occurred");
                ;
            }
        }

        //delete event

        //
    }
}
