using System.Collections.Generic;
using Explore.Cms.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace Explore.Cms.Configuration.OpenApiExamples.Event;

public class UpdateEventRequestExample : OpenApiExample<Events>
{
    public override IOpenApiExample<Events> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiExampleResolver.Resolve("UpdateEvent", new Events
        {
            Id = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
            Name = "Updated Wine Tasting Event",
            Description = "Join us for an exclusive wine tasting experience featuring premium local wines.",
            Date = DateTime.Parse("2024-06-15T19:00:00Z"),
            AvailableSpots = 25,
            Price = 229.99m,
            ImageUrl = "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800",
            GuestIds = new List<Guid>()
        }, namingStrategy));

        return this;
    }
}
