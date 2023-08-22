using Explore.Cms.Configuration;
using Explore.Cms.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;

namespace Explore.Cms.Test.TestUtils;

public static class TestMockFactory
{
    public static IOptions<MongoDbOptions> GetMongoDbOptions()
    {
        var mongoOptions = new MongoDbOptions
        {
            DatabaseName = "test",
            ConnectionString = "mongodb://test"
        };

        return Options.Create(mongoOptions);
    }
    
    public static void SetupMongoRepositoryServices<T>(out IMongoCollection<T> collection,
        out IMongoDatabase database,
        out IMongoClient client,
        out IOptions<MongoDbOptions> options) where T : IDocument, new()
    {
        collection = Substitute.For<IMongoCollection<T>>();
        database = Substitute.For<IMongoDatabase>();
        client = Substitute.For<IMongoClient>();
        options = GetMongoDbOptions();
        
        database.GetCollection<T>(Arg.Any<string>()).Returns(collection);
        client.GetDatabase(Arg.Any<string>()).Returns(database);
    }
}