using Explore.Cms.Configuration;
using FluentValidation.AspNetCore;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Explore.Cms.Startup))]

namespace Explore.Cms;

internal class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = builder.GetContext().Configuration;

        var cosmosDbOptions = config.GetSection("CosmosDB");
        cosmosDbOptions["ConnectionString"] = config.GetConnectionString("CosmosDB");

        builder.Services
            .Configure<MongoDbOptions>(cosmosDbOptions)
            .AddFluentValidationAutoValidation()
            .AddExploreServices();
    }
}