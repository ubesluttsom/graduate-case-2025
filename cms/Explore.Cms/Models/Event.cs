using System.Collections.Generic;
using Explore.Cms.DAL;
using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

[BsonCollection("events")]
public class Events : Document
{
    [BsonElement("name")] public string Name { get; set; } = string.Empty;
    [BsonElement("date")] public DateTime Date { get; set; } = DateTime.Now;
    [BsonElement("availableSpots")] public int AvailableSpots { get; set; } = 0;
    [BsonElement("description")] public string Description { get; set; } = string.Empty;
    [BsonElement("price")] public decimal Price { get; set; } = 0;
    [BsonElement("imageUrl")] public string ImageUrl { get; set; } = string.Empty;
    [BsonElement("guestIds")] public List<Guid> GuestIds { get; init; } = new();
}
