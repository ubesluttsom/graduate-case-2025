using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    ObjectId Id { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { set; }
}