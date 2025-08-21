using Explore.Cms.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace Explore.Cms.Configuration.OpenApiExamples.Event;

public class CreateEventRequestExample : OpenApiExample<Events>
{
    public override IOpenApiExample<Events> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiExampleResolver.Resolve("CreateEvent", new Events
        {
            Name = "Cooking Class",
            Description = "Learn to cook traditional Mediterranean dishes with our chef.",
            Date = DateTime.Parse("2024-07-20T14:00:00Z"),
            AvailableSpots = 15,
            Price = 149.99m,
            ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800"
        }, namingStrategy));

        return this;
    }
}
