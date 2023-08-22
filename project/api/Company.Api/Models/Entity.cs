using System;

namespace Company.Api.Models;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}