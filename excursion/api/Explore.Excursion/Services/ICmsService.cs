using System;
using System.Threading.Tasks;
using Explore.Excursion.Models;

namespace Explore.Excursion.Services;

public interface ICmsService
{
    Task<string> HealthCheckAsync();
    Task<GuestResponse> CreateGuestAsync(CreateGuestRequest guest);
    Task<GuestResponse> GetGuestByIdAsync(Guid userId);
    Task<RoomResponse> GetRoomByIdAsync(Guid userId);
}
