using System;
using System.Collections.Generic;

namespace Company.Api.Models;

public class RoomResponse : Entity
{
    public string RoomNumber { get; init; } = string.Empty;
    public IEnumerable<Guid> GuestIds { get; init; } = new List<Guid>();
    public IEnumerable<Guid> TransactionIds { get; init; } = new List<Guid>();
}