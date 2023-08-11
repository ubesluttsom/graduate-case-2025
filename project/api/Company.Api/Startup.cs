using System;
using Company.Api.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;


[assembly: FunctionsStartup(typeof(Company.Api.Startup))]

namespace Company.Api;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = builder.GetContext().Configuration;

        builder.Services.AddHttpClient<ICmsService, CmsService>(client => client.BaseAddress = new Uri(config["CmsOptions:BaseUrl"]));
    }
}