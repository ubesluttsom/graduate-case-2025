using System.Collections.Generic;
using Explore.Cms.DAL;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

[BsonCollection("rooms")]
public class Room : Document
{
    [BsonElement("roomNumber")] public int RoomNumber { get; set; } = -1;
    [BsonElement("guestIds")] public List<Guid> GuestIds { get; init; } = new();
    [BsonElement("transactionIds")] public List<Guid> TransactionIds { get; init; } = new();
}