using System.Collections.Generic;
using Explore.Cms.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace Explore.Cms.Configuration.OpenApiExamples.Event;

public class EventResponseExample : OpenApiExample<Events>
{
    public override IOpenApiExample<Events> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiExampleResolver.Resolve("Event", new Events
        {
            Id = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
            Name = "Wine Tasting Event",
            Description = "Join us for an exclusive wine tasting experience featuring local wines.",
            Date = DateTime.Parse("2024-06-15T18:00:00Z"),
            AvailableSpots = 20,
            GuestIds = new List<Guid>
            {
                Guid.Parse("87654321-4321-4321-4321-210987654321"),
                Guid.Parse("11111111-1111-1111-1111-111111111111")
            }
        }, namingStrategy));

        return this;
    }
}
