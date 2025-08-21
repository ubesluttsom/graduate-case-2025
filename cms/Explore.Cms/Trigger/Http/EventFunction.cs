using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Explore.Cms.Configuration.OpenApiExamples.Event;
using Explore.Cms.Helpers.Http;
using Explore.Cms.Models;
using Explore.Cms.Services;
using Explore.Cms.Validation.EventValidators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace Explore.Cms.Trigger.Http;

public class EventFunction
{
    private readonly IEventService _eventService;
    private readonly IGuestService _guestService;
    private readonly ILogger<EventFunction> _logger;

    public EventFunction(IEventService eventService, IGuestService guestService, ILogger<EventFunction> logger)
    {
        _eventService = eventService;
        _guestService = guestService;
        _logger = logger;
    }

    [FunctionName("GetEvent")]
    [OpenApiOperation("GetEvent", "Events", Summary = "Get one event", Description = "Get one event")]
    [OpenApiParameter("id", Description = "Id of the event", In = ParameterLocation.Path, Required = true,
        Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Events), Summary = "Ok response",
        Description = "This returns the response", Example = typeof(EventResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the id is not a valid Guid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The not found response",
        Description = "The response when the event is not found")]
    public async Task<IActionResult> GetEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events/{id}")]
        HttpRequest req, string id)
    {
        var parseResult = Guid.TryParse(id, out var guid);
        if (!parseResult) return new BadRequestObjectResult("Invalid id");

        var eventItem = await _eventService.FindOneByIdAsync(guid);

        return eventItem.Id == Guid.Empty ? new NotFoundResult() : new OkObjectResult(eventItem);
    }

    [FunctionName("GetEvents")]
    [OpenApiOperation("GetEvents", "Events", Summary = "Get all events", Description = "Get all events")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(IEnumerable<Events>), Summary = "Ok response",
        Description = "The OK response", Example = typeof(List<EventResponseExample>))]
    public async Task<IActionResult> GetEvents(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events")]
        HttpRequest req)
    {
        return new OkObjectResult(await _eventService.FindAsync(e => true));
    }

    [FunctionName("GetAvailableEvents")]
    [OpenApiOperation("GetAvailableEvents", "Events", Summary = "Get available events", Description = "Get events with available spots")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(IEnumerable<Events>), Summary = "Ok response",
        Description = "The OK response", Example = typeof(List<EventResponseExample>))]
    public async Task<IActionResult> GetAvailableEvents(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "events/available")]
        HttpRequest req)
    {
        return new OkObjectResult(await _eventService.GetAvailableEvents());
    }

    [FunctionName("CreateEvent")]
    [OpenApiOperation("CreateEvent", "Events", Summary = "Create one event", Description = "Create one event")]
    [OpenApiRequestBody("application/json", typeof(Events), Example = typeof(CreateEventRequestExample))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(Events), Summary = "Ok response",
        Description = "This returns the created event", Example = typeof(EventResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the request is invalid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "Conflict response",
        Description = "Conflict response when the event could not be created")]
    public async Task<IActionResult> CreateEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events")]
        HttpRequest req)
    {
        var validatedRequest = await HttpRequestHelpers.ValidateRequest<Events, CreateEventValidator>(req);
        if (!validatedRequest.IsValid) return validatedRequest.ToBadRequest();

        var eventItem = validatedRequest.Value;

        try
        {
            await _eventService.AddOneAsync(eventItem);
            var createdEvent = await _eventService.FindOneByIdAsync(eventItem.Id);
            if (createdEvent.Id == Guid.Empty) return new ConflictObjectResult("Could not create event");

            return new CreatedResult($"events/{createdEvent.Id}", createdEvent);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not create event");
            return new ConflictObjectResult($"Could not create event. Reason: {e.WriteError.Category}");
        }
    }

    [FunctionName("UpdateEvent")]
    [OpenApiOperation("UpdateEvent", "Events", Summary = "Update one event", Description = "Update one event")]
    [OpenApiRequestBody("application/json", typeof(Events), Example = typeof(UpdateEventRequestExample))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Events), Summary = "Ok response",
        Description = "This returns the updated event", Example = typeof(EventResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the request is invalid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The not found response",
        Description = "The response when the event is not found")]
    public async Task<IActionResult> UpdateEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "events")]
        HttpRequest req)
    {
        var validatedRequest = await HttpRequestHelpers.ValidateRequest<Events, UpdateEventValidator>(req);
        if (!validatedRequest.IsValid) return validatedRequest.ToBadRequest();

        var eventItem = validatedRequest.Value;

        var existingEvent = await _eventService.FindOneByIdAsync(eventItem.Id);
        if (existingEvent.Id == Guid.Empty) return new NotFoundResult();

        try
        {
            eventItem = await _eventService.UpdateEvent(eventItem);
            return new OkObjectResult(eventItem);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not update event");
            return new ConflictObjectResult($"Could not update event {eventItem.Id}. Reason: {e.WriteError.Category}");
        }
    }

    [FunctionName("DeleteEvent")]
    [OpenApiOperation("DeleteEvent", "Events", Summary = "Delete one event", Description = "Delete one event")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(string),
        Summary = "The event id", Description = "Id of the event to delete")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK, Summary = "OK response",
        Description = "The event was deleted")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the id is not a valid Guid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The not found response",
        Description = "The response when the event is not found")]
    public async Task<IActionResult> DeleteEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "events/{id}")]
        HttpRequest req, string id)
    {
        var parseResult = Guid.TryParse(id, out var guid);
        if (!parseResult) return new BadRequestObjectResult("Invalid id");

        var eventItem = await _eventService.FindOneByIdAsync(guid);
        if (eventItem.Id == Guid.Empty) return new NotFoundResult();

        if (!(await _eventService.DeleteByIdAsync(guid)))
            return new NotFoundResult();

        return new OkResult();
    }

    [FunctionName("AddGuestToEvent")]
    [OpenApiOperation("AddGuestToEvent", "Events", Summary = "Add guest to event", Description = "Add a guest to an event")]
    [OpenApiParameter("eventId", Description = "Id of the event", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("guestId", Description = "Id of the guest", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Events), Summary = "Ok response",
        Description = "Guest added to event", Example = typeof(EventResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when ids are invalid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "Not found response",
        Description = "Event or guest not found")]
    [OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "Conflict response",
        Description = "Event is full or guest already registered")]
    public async Task<IActionResult> AddGuestToEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "events/{eventId}/guests/{guestId}")]
        HttpRequest req, string eventId, string guestId)
    {
        var eventParseResult = Guid.TryParse(eventId, out var eventGuid);
        var guestParseResult = Guid.TryParse(guestId, out var guestGuid);
        
        if (!eventParseResult || !guestParseResult) 
            return new BadRequestObjectResult("Invalid event or guest id");

        var eventItem = await _eventService.FindOneByIdAsync(eventGuid);
        if (eventItem.Id == Guid.Empty) return new NotFoundObjectResult("Event not found");

        var guest = await _guestService.FindOneByIdAsync(guestGuid);
        if (guest.Id == Guid.Empty) return new NotFoundObjectResult("Guest not found");

        try
        {
            var updatedEvent = await _eventService.AddGuestToEvent(eventGuid, guestGuid);
            return new OkObjectResult(updatedEvent);
        }
        catch (InvalidOperationException ex)
        {
            return new ConflictObjectResult(ex.Message);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not add guest to event");
            return new ConflictObjectResult($"Could not add guest to event. Reason: {e.WriteError.Category}");
        }
    }

    [FunctionName("RemoveGuestFromEvent")]
    [OpenApiOperation("RemoveGuestFromEvent", "Events", Summary = "Remove guest from event", Description = "Remove a guest from an event")]
    [OpenApiParameter("eventId", Description = "Id of the event", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiParameter("guestId", Description = "Id of the guest", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Events), Summary = "Ok response",
        Description = "Guest removed from event", Example = typeof(EventResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when ids are invalid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "Not found response",
        Description = "Event not found")]
    public async Task<IActionResult> RemoveGuestFromEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "events/{eventId}/guests/{guestId}")]
        HttpRequest req, string eventId, string guestId)
    {
        var eventParseResult = Guid.TryParse(eventId, out var eventGuid);
        var guestParseResult = Guid.TryParse(guestId, out var guestGuid);
        
        if (!eventParseResult || !guestParseResult) 
            return new BadRequestObjectResult("Invalid event or guest id");

        var eventItem = await _eventService.FindOneByIdAsync(eventGuid);
        if (eventItem.Id == Guid.Empty) return new NotFoundObjectResult("Event not found");

        try
        {
            var updatedEvent = await _eventService.RemoveGuestFromEvent(eventGuid, guestGuid);
            return new OkObjectResult(updatedEvent);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not remove guest from event");
            return new ConflictObjectResult($"Could not remove guest from event. Reason: {e.WriteError.Category}");
        }
    }
}
