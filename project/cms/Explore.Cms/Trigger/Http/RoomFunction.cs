using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Explore.Cms.Configuration.OpenApiExamples.Room;
using Explore.Cms.Configuration.OpenApiExamples.Transaction;
using Explore.Cms.Models;
using Explore.Cms.Services;
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

public class RoomFunction
{
    private readonly IRoomService _roomService;
    private readonly IGuestService _guestService;
    private readonly ITransactionService _transactionService;
    private readonly ILogger<RoomFunction> _logger;

    public RoomFunction(IRoomService roomRepository, IGuestService guestService, ITransactionService transactionService,
        ILogger<RoomFunction> logger)
    {
        _roomService = roomRepository;
        _guestService = guestService;
        _transactionService = transactionService;
        _logger = logger;
    }

    [FunctionName("GetRoom")]
    [OpenApiOperation("GetRoom", "Rooms", Summary = "Get one room", Description = "Get one room")]
    [OpenApiParameter("id", Description = "Id of the room", In = ParameterLocation.Path, Required = true,
        Type = typeof(ObjectId))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(Room), Summary = "Ok response",
        Description = "This returns the response", Example = typeof(RoomResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the id is not a valid ObjectId")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The not found response",
        Description = "The response when the room is not found")]
    public async Task<IActionResult> GetRoom(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rooms/{id}")]
        HttpRequest req, string id)
    {
        var parseResult = ObjectId.TryParse(id, out var objectId);
        if (!parseResult) return new BadRequestObjectResult("Invalid id");
        
        var room = await _roomService.GetRoom(objectId);

        if (room.Id == ObjectId.Empty) return new NotFoundResult();
        return new OkObjectResult(room);
    }

    [FunctionName("GetRooms")]
    [OpenApiOperation("GetRooms", "Rooms", Summary = "Get all rooms", Description = "Get all rooms")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(IEnumerable<Room>), Summary = "Ok response",
        Description = "The OK response", Example = typeof(List<RoomResponseExample>))]
    public async Task<IActionResult> GetRooms(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rooms")]
        HttpRequest req)
    {
        return new OkObjectResult(await _roomService.GetRooms(r => true));
    }

    [FunctionName("GetRoomTransactions")]
    [OpenApiOperation(nameof(GetRoomTransactions), "Rooms", Summary = "Get all transactions for room", Description = "Get all transactions for specified room")]
    [OpenApiParameter("id", Description = "Id of the room", In = ParameterLocation.Path, Required = true,
        Type = typeof(ObjectId))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(IEnumerable<GuestTransaction>), Summary = "Ok response",
        Description = "The OK response", Example = typeof(List<TransactionResponseExample>))]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = "Bad request response",
        Description = "Bad request response when the id is not a valid ObjectId")]
    public async Task<IActionResult> GetRoomTransactions(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rooms/{id}/transactions")]
        HttpRequest req, string id)
    {
        var parseResult = ObjectId.TryParse(id, out var objectId);
        if (!parseResult) return new BadRequestObjectResult("Invalid id");
        
        var transactions = (await _transactionService.FindAsync(t => t.RoomId == objectId)).ToList();
        return new OkObjectResult(transactions);
    }

    [FunctionName("CreateRoom")]
    [OpenApiOperation("CreateRoom", "Rooms", Summary = "Create one room", Description = "Create one room")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(Room), Summary = "Ok response",
        Description = "This returns the created room", Example = typeof(RoomResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the request is invalid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "Conflict response",
        Description = "Conflict response when the room could not be created")]
    public async Task<IActionResult> CreateRoom(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "rooms")] HttpRequest req)
    {
        var room = new Room()
        {
            RoomNumber = await GetNextNewRoomNumber()
        };

        try
        {
            await _roomService.AddRoom(room);
            return new OkObjectResult(room);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not create new room");
            return new ConflictObjectResult($"Could not create new room. Reason: {e.WriteError.Category}");
        }
    }

    private async Task<int> GetNextNewRoomNumber()
    {
        var rooms = (await _roomService.GetRooms(r => true)).ToList();

        if (!rooms.Any()) return 1;
        return rooms.Select(r => r.RoomNumber).Max() + 1;
    }
}