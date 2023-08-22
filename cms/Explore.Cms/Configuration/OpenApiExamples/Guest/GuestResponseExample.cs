using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace Explore.Cms.Configuration.OpenApiExamples.Guest;

public class GuestResponseExample : OpenApiExample<Models.Guest>
{
    public override IOpenApiExample<Models.Guest> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiExampleResolver.Resolve(
            "Successful response",
            new Models.Guest()
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@email.com",
                RoomId = Guid.NewGuid()
            }, namingStrategy));
        return this;
    }
}