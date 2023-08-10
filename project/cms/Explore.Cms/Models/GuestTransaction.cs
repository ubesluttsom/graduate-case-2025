using Explore.Cms.DAL;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

[BsonCollection("transactions")]
public class GuestTransaction : Document
{
    [BsonElement("amount")] public decimal Amount { get; set; }
    [BsonElement("description")] public string Description { get; set; } = string.Empty;
    [BsonElement("roomId")] public ObjectId RoomId { get; init; } = ObjectId.Empty;
    [BsonElement("guestId")] public ObjectId GuestId { get; init; } = ObjectId.Empty;
    [BsonElement("transactionDate")] public DateTime TransactionDate { get; } = DateTime.Now;
}