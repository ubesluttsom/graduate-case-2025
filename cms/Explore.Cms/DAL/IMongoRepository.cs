using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Explore.Cms.Models;
using MongoDB.Bson;

namespace Explore.Cms.DAL;

public interface IMongoRepository<TDocument> where TDocument : IDocument
{
    void AddOne(TDocument doc);
    Task AddOneAsync(TDocument doc);
    void DeleteOne(Expression<Func<TDocument, bool>> filterExpression);
    Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    TDocument FindOneById(ObjectId id);
    Task<TDocument> FindOneByIdAsync(ObjectId id);
    IEnumerable<TDocument> Find(Expression<Func<TDocument, bool>> filterExpression);
    Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filterExpression);
    TDocument UpdateOne(TDocument document);
    Task<TDocument> UpdateOneAsync(TDocument document);
    bool DeleteById(ObjectId id);
    Task<bool> DeleteByIdAsync(ObjectId id);
}