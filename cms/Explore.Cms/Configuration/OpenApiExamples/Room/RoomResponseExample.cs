using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using MongoDB.Bson;
using Newtonsoft.Json.Serialization;

namespace Explore.Cms.Configuration.OpenApiExamples.Room;

public class RoomResponseExample : OpenApiExample<Models.Room>
{
    public override IOpenApiExample<Models.Room> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiExampleResolver.Resolve(
            "Successful response",
            new Models.Room()
            {
                Id = ObjectId.GenerateNewId(),
                GuestIds = new List<ObjectId>{ ObjectId.GenerateNewId() },
                RoomNumber = 1,
                TransactionIds = new List<ObjectId> { ObjectId.GenerateNewId() },
            }, namingStrategy));
        return this;
    }
}