using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Explore.Cms.Configuration.OpenApiExamples.Guest;
using Explore.Cms.Helpers.Http;
using Explore.Cms.Models;
using Explore.Cms.Services;
using Explore.Cms.Validation.GuestValidators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Explore.Cms.Trigger.Http;

public class GuestFunction
{
    private readonly IRoomService _roomService;
    private readonly IGuestService _guestService;
    private readonly ILogger<GuestFunction> _logger;

    public GuestFunction(IRoomService roomService, IGuestService guestService, ILogger<GuestFunction> logger)
    {
        _roomService = roomService;
        _logger = logger;
        _guestService = guestService;
    }

    [FunctionName("GetGuest")]
    [OpenApiOperation("GetGuest", "Guests", Summary = "Get one guest", Description = "Get one guest")]
    [OpenApiParameter("id", Description = "Id of the guest", In = ParameterLocation.Path, Required = true,
        Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Guest), Summary = "Ok response",
        Description = "This returns the response", Example = typeof(GuestResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the id is not a valid Guid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The not found response",
        Description = "The response when the guest is not found")]
    public async Task<IActionResult> GetGuest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "guests/{id}")]
        HttpRequest req, string id)
    {
        var tryParse = Guid.TryParse(id, out var guid);
        if (!tryParse) return new BadRequestObjectResult("Invalid id");
        
        var guest = await _guestService.FindOneByIdAsync(guid);

        if (guest.Id == Guid.Empty) return new NotFoundResult();
        return new OkObjectResult(guest);
    }

    [FunctionName("GetGuests")]
    [OpenApiOperation("GetGuests", "Guests", Summary = "Get all guests", Description = "Get all guests")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(IEnumerable<Guest>), Summary = "Ok response",
        Description = "The OK response", Example = typeof(List<GuestResponseExample>))]
    public async Task<IActionResult> GetGuests(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "guests")]
        HttpRequest req)
    {
        return new OkObjectResult(await _guestService.FindAsync(g => true));
    }

    [FunctionName("UpdateGuest")]
    [OpenApiOperation("UpdateGuest", "Guests", Summary = "Update one guest", Description = "Update one guest")]
    [OpenApiRequestBody("application/json", typeof(Guest), Example = typeof(UpdateGuestRequestExample))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Guest), Summary = "Ok response",
        Description = "This returns the updated guest", Example = typeof(GuestResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the request is invalid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The not found response",
        Description = "The response when the guest is not found")]
    public async Task<IActionResult> UpdateGuest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "guests")]
        HttpRequest req)
    {
        var validatedRequest = await HttpRequestHelpers.ValidateRequest<Guest, UpdateGuestValidator>(req);
        if (!validatedRequest.IsValid) return validatedRequest.ToBadRequest();
        
        var guest = validatedRequest.Value;

        var existingGuest = await _guestService.FindOneByIdAsync(guest.Id);
        
        if (existingGuest.Id == Guid.Empty) return new NotFoundResult();
        if (guest.RoomId != existingGuest.RoomId) return new BadRequestObjectResult("Cannot change guest room");

        try
        {
            guest = await _guestService.UpdateGuest(guest);
            return new OkObjectResult(guest);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not update guest");
            return new ConflictObjectResult($"Could not update guest {guest.Id}. Reason: {e.WriteError.Category}");
        }
    }

    [FunctionName("CreateGuest")]
    [OpenApiOperation("CreateGuest", "Guests", Summary = "Create one guest", Description = "Create one guest")]
    [OpenApiRequestBody("application/json", typeof(Guest), Example = typeof(CreateGuestRequestExample))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(Guest), Summary = "Ok response",
        Description = "This returns the created guest", Example = typeof(GuestResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the request is invalid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "Conflict response",
        Description = "Conflict response when the guest could not be created")]
    public async Task<IActionResult> CreateGuest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "guests")]
        HttpRequest req)
    {
        var validatedRequest = await HttpRequestHelpers.ValidateRequest<Guest, CreateGuestValidator>(req);
        if (!validatedRequest.IsValid) return validatedRequest.ToBadRequest();

        var guest = validatedRequest.Value;
        
        try
        {
            var room = await _roomService.GetNextAvailableRoom();
            await _roomService.AddGuestToRoom(room.Id, guest.Id);
            guest.RoomId = room.Id;
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not assign guest to a room");
            return new ConflictObjectResult($"Could not assign guest {guest.Id} to a room. Reason: {e.WriteError.Category}");
        }

        try
        {
            await _guestService.AddOneAsync(guest);
            var createdGuest = await _guestService.FindOneByIdAsync(guest.Id);
            if (createdGuest.Id == Guid.Empty)
                return new ConflictObjectResult($"Could not create guest {guest.Id}");
            return new CreatedResult($"guest/{guest.Id}", createdGuest);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not create guest");
            return new ConflictObjectResult($"Could not create guest {guest.Id}. Reason: {e.WriteError.Category}");
        }
    }

    [FunctionName("DeleteGuest")]
    [OpenApiOperation("DeleteGuest", "Guests", Summary = "Delete one guest", Description = "Delete one guest")]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(string),
        Summary = "The guest id", Description = "Id of the guest to delete")]
    [OpenApiResponseWithoutBody(HttpStatusCode.OK, Summary = "OK response",
        Description = "The guest was deleted")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the id is not a valid Guid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The not found response",
        Description = "The response when the guest is not found")]
    public async Task<IActionResult> DeleteGuest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "guests/{id}")]
        HttpRequest req, string id)
    {
        var parseResult = Guid.TryParse(id, out var guid);
        if (!parseResult) return new BadRequestObjectResult("Invalid id");
        
        var guest = await _guestService.FindOneByIdAsync(guid);
        
        if (!(await _guestService.DeleteByIdAsync(guest.Id)))
            return new NotFoundResult();

        await _roomService.RemoveGuestFromRoom(guest.RoomId, guest.Id);
        
        return new OkResult();
    }
}