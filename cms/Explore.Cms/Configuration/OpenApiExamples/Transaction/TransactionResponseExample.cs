using Explore.Cms.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using MongoDB.Bson;
using Newtonsoft.Json.Serialization;

namespace Explore.Cms.Configuration.OpenApiExamples.Transaction;

public class TransactionResponseExample : OpenApiExample<GuestTransaction>
{
    public override IOpenApiExample<GuestTransaction> Build(NamingStrategy? namingStrategy = null)
    {
        Examples.Add(OpenApiExampleResolver.Resolve(
            "Successful response",
            new GuestTransaction()
            {
                Id = Guid.NewGuid(),
                Amount = new decimal(100.99),
                Description = "Example transaction",
                GuestId = Guid.NewGuid(),
                RoomId = Guid.NewGuid(),
            }, namingStrategy));
        return this;
    }
}