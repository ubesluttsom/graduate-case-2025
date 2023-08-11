using System;

namespace Company.Api.Models;

public class GuestResponse : Entity
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid UserId { get; set; } = new();
    public string RoomId { get; set; } = string.Empty;
}