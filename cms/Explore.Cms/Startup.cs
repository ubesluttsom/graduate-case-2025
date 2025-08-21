using System.Threading.Tasks;
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

        // Seed database for development
        if (config.GetValue<bool>("SeedDatabase", true))
        {
            if (cosmosDbOptions["ConnectionString"] == null || cosmosDbOptions["DatabaseName"] == null)
            {
                throw new System.Exception("CosmosDB connection string or database name is not configured.");
            }
            _ = Task.Run(async () => await SimpleSeed.SeedDatabase(
                        cosmosDbOptions["ConnectionString"]!,
                        cosmosDbOptions["DatabaseName"]!
                        ));
        }
    }
}
