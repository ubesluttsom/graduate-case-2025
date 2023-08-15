using System.Net;
using Explore.Cms.Configuration.OpenApiExamples;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace Explore.Cms.Trigger.Http;

public class Health
{
    private readonly ILogger<Health> _logger;

    public Health(ILogger<Health> logger)
    {
        _logger = logger;
    }

    [FunctionName("Health")]
    [OpenApiOperation("Health", "Health", Summary = "Get health status", Description = "Get health status")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = "The response",
        Description = "This returns the response", Example = typeof(HealthResponseExample))]
    public IActionResult RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request");
        return new OkObjectResult("Up and running!");
    }
}