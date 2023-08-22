using Explore.Cms.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Explore.Cms.Configuration;

public static class ExploreExtensions
{
    public static IServiceCollection AddExploreServices(this IServiceCollection services)
    {
        
        services
            .AddSingleton<IMongoClient>(s =>
            {
                var connectionString = s.GetRequiredService<IConfiguration>().GetConnectionString("CosmosDB");
                
                return new MongoClient(connectionString);
            })
            .AddScoped<IGuestService, GuestService>()
            .AddScoped<IRoomService, RoomService>()
            .AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}