using System;
using System.Net;
using System.Threading.Tasks;
using Explore.Excursion.Exceptions;
using Explore.Excursion.Helpers.Http;
using Explore.Excursion.Models;
using Explore.Excursion.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Explore.Excursion.Functions;

public class RoomFunction
{
    private readonly ICmsService _cmsService;
    private readonly ILogger<RoomFunction> _logger;
    
    public RoomFunction(ICmsService cmsService, ILogger<RoomFunction> logger)
    {
        _cmsService = cmsService;
        _logger = logger;
    }
    
    [FunctionName("GetRoom")]
    [OpenApiOperation("GetRoom", "GetRoom", Summary = "Get one room", Description = "Get one room by its id")]
    [OpenApiParameter("id", Description = "Id of the room", In = ParameterLocation.Path, Required = true,
        Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(RoomResponse), Summary = "The ok response",
        Description = "The ok response")]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(string),
        Summary = "Bad request response",
        Description = "Bad request response when the request is not valid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway, Summary = "Bad gateway response",
        Description = "Bad gateway response when the api could not correctly forward the request")]
    public async Task<IActionResult> GetRoomByIdAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rooms/{id}")] HttpRequest req, string id)
    {
        try
        {
            var isParsed = Guid.TryParse(id, out var parsedGuid);
            if (!isParsed) throw new ValidationException("Id is not a valid guid");
            
            var response = await _cmsService.GetRoomByIdAsync(parsedGuid);

            return new OkObjectResult(response);
        }
        catch (ValidationException e)
        {
            return HttpHelpers.GetObjectResult(HttpStatusCode.BadRequest, e.Message);
        }
        catch (CmsException e)
        {
            _logger.LogError(e, "{@Message}", e.Message);
            return HttpHelpers.GetObjectResult(HttpStatusCode.BadGateway);
        }
    }
}
