using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Explore.Cms.Configuration;
using Explore.Cms.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Explore.Cms.DAL;

public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument, new()
{
    protected readonly IMongoCollection<TDocument> Collection;

    protected MongoRepository(IOptions<MongoDbOptions> options)
    {
        var database = new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName);
        Collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
    }

    private static string GetCollectionName(ICustomAttributeProvider documentType)
    {
        return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                typeof(BsonCollectionAttribute),
                true)
            .First()).CollectionName;
    }

    public void AddOne(TDocument doc)
    {
        Collection.InsertOne(doc);
    }

    public async Task AddOneAsync(TDocument doc)
    {
        await Collection.InsertOneAsync(doc, new InsertOneOptions());
    }

    public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        Collection.FindOneAndDelete(filterExpression);
    }

    public async Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        await Collection.FindOneAndDeleteAsync(filterExpression);
    }

    public TDocument FindOneById(Guid id)
    {
        var res = Collection.Find(e => e.Id.Equals(id));

        return res.Any() ? res.First() : new TDocument();
    }

    public async Task<TDocument> FindOneByIdAsync(Guid id)
    {
        var res = (await Collection.FindAsync(e => e.Id.Equals(id))).ToList();

        return res.Any() ? res.First() : new TDocument();
    }

    public IEnumerable<TDocument> Find(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Collection.Find(filterExpression).ToEnumerable();
    }

    public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return (await Collection.FindAsync(filterExpression)).ToEnumerable();
    }

    public TDocument UpdateOne(TDocument document)
    {
        document.UpdatedAt = DateTime.Now;
        
        return Collection.FindOneAndUpdate(r => r.Id.Equals(document.Id),
            new ObjectUpdateDefinition<TDocument>(document));
    }

    public async Task<TDocument> UpdateOneAsync(TDocument document)
    {
        document.UpdatedAt = DateTime.Now;
        
        return await Collection.FindOneAndUpdateAsync<TDocument>(r => r.Id == document.Id,
            new ObjectUpdateDefinition<TDocument>(document),
            new FindOneAndUpdateOptions<TDocument, TDocument>
                { IsUpsert = false, ReturnDocument = ReturnDocument.After });
    }

    public bool DeleteById(Guid id)
    {
        return Collection.FindOneAndDelete(d => d.Id.Equals(id)) != null;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        var deletedDocument = await Collection.FindOneAndDeleteAsync(d => d.Id.Equals(id));
        return deletedDocument != null;
    }
}