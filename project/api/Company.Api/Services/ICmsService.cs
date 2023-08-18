using System;
using System.Threading.Tasks;
using Company.Api.Models;

namespace Company.Api.Services;

public interface ICmsService
{
    Task<string> HealthCheckAsync();
    Task<GuestResponse> CreateGuestAsync(CreateGuestRequest guest);
    Task<GuestResponse> GetGuestByIdAsync(Guid userId);
}
