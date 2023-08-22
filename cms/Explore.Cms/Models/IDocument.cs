namespace Explore.Cms.Models;

public interface IDocument
{

    Guid Id { get; set; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { set; }
}