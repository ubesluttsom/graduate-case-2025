using Explore.Cms.DAL;
using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

[BsonCollection("transactions")]
public class GuestTransaction : Document
{
    [BsonElement("amount")] public decimal Amount { get; set; }
    [BsonElement("description")] public string Description { get; set; } = string.Empty;
    [BsonElement("roomId")] public Guid RoomId { get; init; } = Guid.Empty;
    [BsonElement("guestId")] public Guid GuestId { get; init; } = Guid.Empty;
    [BsonElement("transactionDate")] public DateTime TransactionDate { get; } = DateTime.Now;
}