using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Explore.Cms.Configuration;

public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
{
    public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;

    public override OpenApiInfo Info { get; set; } = new()
    {
        Title = "Explore Cruise Management System (CMS)",
        Version = "1.0.0",
        Description = "This is the documentation for the CMS API",
    };
}
 