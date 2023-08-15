using Explore.Cms.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Explore.Cms.Configuration;

public static class ExploreExtensions
{
    public static IServiceCollection AddExploreServices(this IServiceCollection services)
    {
        services
            .AddScoped<IGuestService, GuestService>()
            .AddScoped<IRoomService, RoomService>()
            .AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}