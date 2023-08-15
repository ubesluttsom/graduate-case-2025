using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace Explore.Cms.Configuration.OpenApiExamples.Guest;

public class CreateGuestRequestExample : OpenApiExample<CreateGuestRequest>
{
    public override IOpenApiExample<CreateGuestRequest> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiExampleResolver.Resolve(
            "Create guest request",
            new CreateGuestRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@email.com",
            }, namingStrategy));
        return this;
    }
}

public class CreateGuestRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}