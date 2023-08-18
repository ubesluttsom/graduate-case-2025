using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Explore.Cms.Models;

public interface IDocument
{

    Guid Id { get; set; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { set; }
}