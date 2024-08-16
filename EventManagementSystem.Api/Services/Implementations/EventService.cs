using EventManagementSystem.Api.Common;
using EventManagementSystem.Api.Common.EventModels;
using EventManagementSystem.Api.Data;
using EventManagementSystem.Api.Data.Models;
using EventManagementSystem.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManagementSystem.Api.Services.Implementations;

public class EventService: IEventService
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public EventService(AppDbContext context, UserManager<User> userManager, IEmailService emailService)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<Result<string>> CreateEvent(CreateEventRequest request, string organizerId)
    {
        var organizer =await _userManager.FindByIdAsync(organizerId);
        if(organizer is null)
            return Result.Fail<string>("Invalid Organizer");

        var existingEvent = _context.Events.FirstOrDefault(x => x.Title.ToLower() == request.Title.ToLower() && x.UserId.ToString() == organizerId && !x.Deleted);
        if (existingEvent is not null)
            return Result.Fail<string>("Event already exists!");

        var newEvent = new Event()
        {
            Title = request.Title,
            Description = request.Description,
            Location = request.Location,
            MaxParticipants = request.MaxParticipants,
            Date = request.Date,
            DateCreated = DateTime.Now,
            UserId = int.Parse(organizerId)
        };

        await _context.Events.AddAsync(newEvent);
        await _context.SaveChangesAsync();
        return Result.Ok<string>("Event Created Successfully");
    }

    public async Task<Result<string>> DeleteEvent(int eventId, string organizerId, bool isAdmin=false)
    {
        var organizer = await _userManager.FindByIdAsync(organizerId);
        if (organizer is null)
            return Result.Fail<string>("Invalid Organizer");
        Event? existingEvent;
        if (!isAdmin)
        {
            existingEvent = _context.Events.FirstOrDefault(x => x.Id == eventId && !x.Deleted && x.UserId == int.Parse(organizerId));
        }
        else
        {
            existingEvent = _context.Events.FirstOrDefault(x => x.Id == eventId && !x.Deleted);
        }
        if (existingEvent is null)
            return Result.Fail<string>("Invalid Event");

        existingEvent.Deleted = true;
        _context.Events.Update(existingEvent);
        await _context.SaveChangesAsync();
        return Result.Ok<string>("Event Deleted Successfully");
    }

    public async Task<Result<string>> UpdateEvent(UpdateEventRequest request, string organizerId, bool isAdmin=false)
    {
        var organizer = await _userManager.FindByIdAsync(organizerId);
        if (organizer is null)
            return Result.Fail<string>("Invalid Organizer");

        Event? existingEvent;
        if (!isAdmin)
        {
            existingEvent = _context.Events.FirstOrDefault(x => x.Id == request.EventId && !x.Deleted && x.UserId == int.Parse(organizerId));
        }
        else
        {
            existingEvent = _context.Events.FirstOrDefault(x => x.Id == request.EventId && !x.Deleted);
        }
        if (existingEvent is null)
            return Result.Fail<string>("Invalid Event");

        existingEvent.Title = request.Title;
        existingEvent.Description = request.Description;
        existingEvent.Location = request.Location;
        existingEvent.Date = request.Date;
        existingEvent.MaxParticipants = request.MaxParticipants;
        _context.Events.Update(existingEvent);
        await _context.SaveChangesAsync();
        return Result.Ok<string>("Event Updated Successfully");
    }

    public async Task<Result<string>> RegisterForEvent(int eventId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Fail<string>("Invalid Organizer");

        var existingEvent = _context.Events.FirstOrDefault(x => x.Id == eventId && !x.Deleted);
        if (existingEvent is null)
            return Result.Fail<string>("Invalid Event");

        var registrations = _context.EventRegistrations.Where(x => x.EventId == eventId).ToList();

        if (registrations.Exists(x => x.UserId.ToString() == userId))
            return Result.Fail<string>("Event already Registered");

        else if (registrations.Count == existingEvent.MaxParticipants)
            return Result.Fail<string>("Participant limit Exceeded");

        var eventRegistration = new EventRegistration()
        {
            UserId = int.Parse(userId),
            EventId = eventId,
            DateRegistered = DateTime.Now,
        };
        await _context.EventRegistrations.AddAsync(eventRegistration);
        await _context.SaveChangesAsync();
        _emailService.SendEventRegistrationEmail(user.FirstName, user.Email, existingEvent.Title);
        return Result.Ok<string>("Event Registration Successful");

    }

    public async Task<Result<GetEventResponse>> GetEventResponse(string? userId, int? eventId, bool isAdminOrUser=false)
    {
        //admin/user get all events
        if(string.IsNullOrEmpty(userId) && eventId is null && isAdminOrUser)
        {
            var allEventsForAllUsers = _context.Events.ToList();
            var response = new GetEventResponse() { Events = allEventsForAllUsers };
            return Result.Ok<GetEventResponse>(response);
        }
        //admin/user get single event
        else if(string.IsNullOrEmpty(userId) && eventId is not null && isAdminOrUser)
        {
            var singleevent = _context.Events.Where(x => x.Id == eventId).First();
            var response = new GetEventResponse() { SingleEvent = singleevent };
            return Result.Ok<GetEventResponse>(response);
        }

        // admin/organiser get all  events for user
        else if(!string.IsNullOrEmpty(userId) && eventId is null)
        {
            var organizer = await _userManager.FindByIdAsync(userId);
            if (organizer is null)
                return Result.Fail<GetEventResponse>("Invalid User");
            var allEventsForSingleUser = _context.Events.Where(x => x.UserId.ToString() == userId).ToList();
            var response = new GetEventResponse() { Events = allEventsForSingleUser };
            return Result.Ok<GetEventResponse>(response);
        }
        // admin/organiser get single event for user
        else if(!string.IsNullOrEmpty(userId) && eventId is not null)
        {
            var organizer = await _userManager.FindByIdAsync(userId);
            if (organizer is null)
                return Result.Fail<GetEventResponse>("Invalid User");

            var singleEventForSingleUser = _context.Events.Where(x => x.UserId.ToString() == userId && x.Id == eventId).SingleOrDefault();
            if(singleEventForSingleUser is null)
                Result.Fail<GetEventResponse>("Invalid Event or User");
            var response = new GetEventResponse() { SingleEvent = singleEventForSingleUser };
            return Result.Ok<GetEventResponse>(response);
        }
        else
        {
            return Result.Fail<GetEventResponse>("Cannot Process Request");
        }
    }

    public async Task<Result<List<GetRegistrationsResponse>>> GetRegistrations()
    {
        var registrations = _context.EventRegistrations.Include(x => x.User).Include(x =>x.Event).ToList();
        var response = registrations.Select(x => new GetRegistrationsResponse() { EmailAddres = x.User.Email, EventTitle = x.Event.Title }).ToList();
        return Result.Ok<List<GetRegistrationsResponse>>(response);
    }
}
