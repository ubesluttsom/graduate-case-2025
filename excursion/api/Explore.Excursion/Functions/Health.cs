using System.Net;
using System.Threading.Tasks;
using Explore.Excursion.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace Explore.Excursion.Functions;

public class Health
{
    private readonly ICmsService _cmsService;
    
    public Health(ICmsService cmsService)
    {
        _cmsService = cmsService;
    }
    
    [FunctionName("Health")]
    [OpenApiOperation("Health",
        "Health",
        Summary = "Get health status",
        Description = "Get health status for api. Also gets the health status of the CMS.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(string), Summary = "The response",
        Description = "This returns the response")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
    {
        var healthResponse = await _cmsService.HealthCheckAsync();

        var response = "Up and running!\n\n" +
                       $"CMS status: {healthResponse}";
        
        return new OkObjectResult(response);
    }
}
