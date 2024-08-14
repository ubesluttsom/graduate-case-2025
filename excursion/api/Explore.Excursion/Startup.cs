using System;
using Explore.Excursion.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(Explore.Excursion.Startup))]

namespace Explore.Excursion;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = builder.GetContext().Configuration;

        builder.Services.AddHttpClient<ICmsService, CmsService>(client => client.BaseAddress = new Uri(config["CmsOptions:BaseUrl"]));
    }
}