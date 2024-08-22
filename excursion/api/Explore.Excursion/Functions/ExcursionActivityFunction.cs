using System;
using System.Net;
using System.Threading.Tasks;
using Explore.Excursion.Exceptions;
using Explore.Excursion.Helpers.Http;
using Explore.Excursion.Models;
using Explore.Excursion.Services;
using Explore.Excursion.Validation.GuestValidators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ValidationException = FluentValidation.ValidationException;

namespace Explore.Excursion.Functions;

public class ExcursionActivityFunction
{

    private readonly ICmsService _cmsService;
    private readonly ILogger _logger;
    public ExcursionActivityFunction(ICmsService cmsService)
    {
        _cmsService = cmsService;
    }


    [FunctionName("GetExcursion")]
    [OpenApiOperation("GetExcursion", "GetExcursion", Summary = "Get an excursion")]
    public async Task<IActionResult> GetExcursion(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "excursion")] HttpRequest req)
    {


        var response = await _cmsService.GetExcursionActivity();
        return new OkObjectResult(response);

    }

    [FunctionName("GetExcursions")]
    [OpenApiOperation("GetExcursions", "GetExcursions", Summary = "Get all excursion")]
    public async Task<IActionResult> GetAllExcursions(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "excursions")] HttpRequest req)
    {


        var response = await _cmsService.GetExcursionActivities();
        return new OkObjectResult(response);

    }



}
