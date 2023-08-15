using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

public class Document : IDocument
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.Empty;
    [BsonElement("createdAt")] public DateTime CreatedAt => Id.CreationTime;
    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; } = DateTime.Now;
}