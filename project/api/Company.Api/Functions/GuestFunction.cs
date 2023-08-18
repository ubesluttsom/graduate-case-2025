using System;
using System.Net;
using System.Threading.Tasks;
using Company.Api.Exceptions;
using Company.Api.Helpers.Http;
using Company.Api.Models;
using Company.Api.Services;
using Company.Api.Validation.GuestValidators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ValidationException = FluentValidation.ValidationException;

namespace Company.Api.Functions;

public class GuestFunction
{
    private readonly ICmsService _cmsService;
    private readonly ILogger<GuestFunction> _logger;
    
    public GuestFunction(ICmsService cmsService, ILogger<GuestFunction> logger)
    {
        _cmsService = cmsService;
        _logger = logger;
    }
    
    [FunctionName("CreateGuest")]
    [OpenApiOperation("CreateGuest", "CreateGuest", Summary = "Create a guest", Description = "Create a guest")]
    [OpenApiRequestBody("application/json", typeof(CreateGuestRequest), Description = "The guest to be created")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(GuestResponse), Summary = "The ok response",
        Description = "The ok response")]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = "Bad request response",
        Description = "Bad request response when the request is not valid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway, Summary = "Bad gateway response",
        Description = "Bad gateway response when the api could not correctly forward the request")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.User, "post", Route = "guests")] HttpRequest req)
    {
        try
        {
            var validatedRequest =
                await HttpHelpers.ValidateRequest<CreateGuestRequest, CreateGuestRequestValidator>(req);
            var response = await _cmsService.CreateGuestAsync(validatedRequest);
            
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

    [FunctionName("GetGuest")]
    [OpenApiOperation("GetGuest", "GetGuest", Summary = "Get one guest", Description = "Get one guest by their id")]
    [OpenApiParameter("id", Description = "Id of the guest", In = ParameterLocation.Path, Required = true,
        Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(GuestResponse), Summary = "The ok response",
        Description = "The ok response")]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(string), Summary = "Bad request response",
        Description = "Bad request response when the request is not valid")]
    [OpenApiResponseWithoutBody(HttpStatusCode.BadGateway, Summary = "Bad gateway response",
        Description = "Bad gateway response when the api could not correctly forward the request")]
    public async Task<IActionResult> GetGuestByIdAsync([HttpTrigger(AuthorizationLevel.User, "get", Route = "guests/{id}")] HttpRequest req, string id)
    {
        try
        {
            var response = await _cmsService.GetGuestByIdAsync(Guid.Parse(id));
            
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
