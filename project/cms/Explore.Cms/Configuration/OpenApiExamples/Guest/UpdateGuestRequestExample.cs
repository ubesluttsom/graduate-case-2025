using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Explore.Cms.Configuration.OpenApiExamples.Guest;

public class UpdateGuestRequestExample : OpenApiExample<UpdateGuestRequest>
{
    public override IOpenApiExample<UpdateGuestRequest> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiExampleResolver.Resolve(
            "Update guest request",
            new UpdateGuestRequest()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "John.Doe@email.com",
                Id = ObjectId.GenerateNewId(),
                RoomId = ObjectId.GenerateNewId(),
                UserId = Guid.NewGuid()
            }, namingStrategy));
        return this;
    }
}

public class UpdateGuestRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [JsonProperty("_id")]  public ObjectId Id { get; set; } = ObjectId.Empty;
    public ObjectId RoomId { get; set; } = ObjectId.Empty;
    public Guid UserId { get; set; } = new();
}