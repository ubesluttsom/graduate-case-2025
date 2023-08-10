using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Explore.Cms.Models;

public class Document : IDocument
{
    [BsonId]
    [JsonProperty("_id")]
    public ObjectId Id { get; set; } = ObjectId.Empty;
    [BsonElement("createdAt")] public DateTime CreatedAt => Id.CreationTime;
    [BsonElement("updatedAt")] public DateTime UpdatedAt { get; set; } = DateTime.Now;
}