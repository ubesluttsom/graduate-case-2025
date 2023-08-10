using Explore.Cms.DAL;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

[BsonCollection("guests")]
public class Guest : Document
{
    [BsonElement("firstName")] public string FirstName { get; init; } = string.Empty;
    [BsonElement("lastName")] public string LastName { get; init; } = string.Empty;
    [BsonElement("email")] public string Email { get; set; } = string.Empty;
    [BsonElement("roomId")] public ObjectId RoomId { get; set; } = ObjectId.Empty;
    [BsonElement("userId")] public Guid UserId { get; set; } = new();
}