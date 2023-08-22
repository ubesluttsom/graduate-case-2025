using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

public class Document : IDocument
{
    [BsonId]
    public Guid Id { get; set; } = Guid.Empty;
    [BsonElement("createdAt")] public DateTime CreatedAt => DateTime.Now;
    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; } = DateTime.Now;
}