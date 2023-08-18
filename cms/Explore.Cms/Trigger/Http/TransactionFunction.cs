using System.Net;
using System.Threading.Tasks;
using Explore.Cms.Configuration.OpenApiExamples.Transaction;
using Explore.Cms.Helpers.Http;
using Explore.Cms.Models;
using Explore.Cms.Services;
using Explore.Cms.Validation.TransactionValidators;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Explore.Cms.Trigger.Http;

public class TransactionFunction
{
    private readonly ITransactionService _transactionService;
    private readonly IRoomService _roomService;
    private readonly ILogger<TransactionFunction> _logger;

    public TransactionFunction(ITransactionService transactionService, IRoomService roomService,
        ILogger<TransactionFunction> logger)
    {
        _transactionService = transactionService;
        _roomService = roomService;
        _logger = logger;
    }

    [FunctionName("GetTransaction")]
    [OpenApiOperation("GetTransaction", "Transactions", Summary = "Get one transaction", Description = "Get one transaction")]
    [OpenApiParameter("id", Description = "Id of the transaction", In = ParameterLocation.Path, Required = true,
        Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(GuestTransaction), Summary = "Ok response",
        Description = "This returns the response", Example = typeof(TransactionResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the id is not a valid Guid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound, Summary = "The not found response",
        Description = "The response when the transaction is not found")]
    public async Task<IActionResult> GetTransaction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "transactions/{id}")]
        HttpRequest req, string id)
    {
        var parseResult = Guid.TryParse(id, out var guid);
        if (!parseResult) return new BadRequestObjectResult("Invalid id");
        
        var transaction = await _transactionService.FindOneByIdAsync(guid);

        return transaction.Id == Guid.Empty ? new NotFoundResult() : new OkObjectResult(transaction);
    }
    

    [FunctionName("CreateTransaction")]
    [OpenApiOperation("CreateTransaction", "Transactions", Summary = "Create one transaction", Description = "Create one transaction")]
    [OpenApiRequestBody("application/json", typeof(GuestTransaction), Example = typeof(CreateTransactionRequestExample))]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(GuestTransaction), Summary = "Ok response",
        Description = "This returns the created transaction", Example = typeof(TransactionResponseExample))]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Summary = "Bad request response",
        Description = "Bad request response when the request is invalid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.Conflict, Summary = "Conflict response",
        Description = "Conflict response when the transaction could not be created")]
    public async Task<IActionResult> CreateTransaction(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "transactions")]
        HttpRequest req)
    {
        var validatedRequest =
            await HttpRequestHelpers.ValidateRequest<GuestTransaction, CreateTransactionValidator>(req);
        if (!validatedRequest.IsValid) return validatedRequest.ToBadRequest();

        var transaction = validatedRequest.Value;

        var room = await _roomService.FindOneByIdAsync(transaction.RoomId);
        if (room.Id == Guid.Empty) return new NotFoundObjectResult("Room does not exist.");


        try
        {
            await _roomService.AddTransactionToRoom(room.Id, transaction.Id);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not add transaction to room {RoomId}", room.Id);
            return new ConflictObjectResult($"Could not add transaction to room {room.Id}. Reason: {e.WriteError.Category}");
        }

        try
        {
            await _transactionService.AddOneAsync(transaction);
        }
        catch (MongoWriteException e)
        {
            _logger.LogError(e, "Could not create transaction");
            return new ConflictObjectResult($"Could not create transaction. Reason: {e.WriteError.Category}");
        }
        
        var createdTransaction = await _transactionService.FindOneByIdAsync(transaction.Id);
        if (createdTransaction.Id == Guid.Empty) return new ConflictObjectResult("Could not create transaction");

        return new CreatedResult($"guest/{createdTransaction.Id}", createdTransaction);
    }
}