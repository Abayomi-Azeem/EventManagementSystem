using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Api.Data;
using EventManagementSystem.Api.Data.Models;
using EventManagementSystem.Api.Services.Interfaces;
using EventManagementSystem.Api.Common.EventModels;
using Azure.Core;

namespace EventManagementSystem.Api.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = TempData["UserId"].ToString();            
            var events =await _eventService.GetEventResponse(userId, null);
            TempData["UserId"] = userId;
            return View(events.Value);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string userId = TempData["UserId"].ToString();
            var events = await _eventService.GetEventResponse(userId, id);
            TempData["UserId"] = userId;
            return View(events.Value);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventRequest request)
        {
            string userId = TempData["UserId"].ToString();

            var eventCreated =await _eventService.CreateEvent(request,userId);
            TempData["UserId"] = userId;
            if (eventCreated.Success)
                return RedirectToAction(nameof(Index));
            return BadRequest(eventCreated.Error);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string userId = TempData["UserId"].ToString();
            var @event =await _eventService.GetEventResponse(userId, id);
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
            TempData["UserId"] = userId;
            return View(updateReq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateEventRequest request)
        {
            string userId = TempData["UserId"].ToString();
            var eventId = ViewBag.EventId;
            var eventEdited = await _eventService.UpdateEvent(request, userId);
            TempData["UserId"] = userId;
            if (eventEdited.IsFailure)
            {
                return BadRequest(eventEdited.Error);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            string userId = TempData["UserId"].ToString();
            var @event = await _eventService.GetEventResponse(userId, id);
            if (@event.IsFailure)
            {
                return NotFound();
            }
            TempData["UserId"] = userId;
            return View(@event.Value);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userId = TempData["UserId"].ToString();

            var eventdeleted = await _eventService.DeleteEvent(id, userId);
            TempData["UserId"] = userId;

            if (eventdeleted.IsFailure)
            {
                return BadRequest(eventdeleted.Error);
            }
            return RedirectToAction(nameof(Index));
            
        }
       
    }
}
