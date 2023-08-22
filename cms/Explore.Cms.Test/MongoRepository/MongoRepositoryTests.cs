using System.Linq.Expressions;
using Explore.Cms.Configuration;
using Explore.Cms.DAL;
using Explore.Cms.Models;
using Explore.Cms.Test.TestUtils;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;

namespace Explore.Cms.Test.MongoRepository;

public abstract class MongoRepositoryTests<T> where T : IDocument, new()
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<T> _collection;
    private readonly IMongoClient _client;
    private readonly IOptions<MongoDbOptions> _options;
    
    private readonly IMongoRepository<T> _repository;
    
    protected MongoRepositoryTests()
    {
        TestMockFactory.SetupMongoRepositoryServices(out _collection, out _database, out _client, out _options);
        
        _repository = new MongoRepository<T>(_options, _client);
    }
    
    [Fact]
    public void Constructor_ShouldCallGetDatabase()
    {
        // Assert
        _client.Received(1).GetDatabase(Arg.Any<string>());
    }
    
    [Fact]
    public void Constructor_ShouldCallGetCollection()
    {
        // Assert
        _database.Received(1).GetCollection<T>(Arg.Any<string>());
    }

    [Fact]
    public void AddOneAsync_ShouldCallInsertOneAsync()
    {
        // Arrange
        var doc = new T();
        
        // Act
        _repository.AddOneAsync(doc);
        
        // Assert
        _collection.Received(1).InsertOneAsync(doc, Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public void AddOne_ShouldCallInsertOne()
    {
        // Arrange
        var doc = new T();
        
        // Act
        _repository.AddOne(doc);
        
        // Assert
        _collection.Received(1).InsertOne(doc, Arg.Any<InsertOneOptions>(), Arg.Any<CancellationToken>());
    }
}