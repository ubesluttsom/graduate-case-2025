using System.Collections.Generic;
using System.Threading.Tasks;
using Explore.Cms.DAL;
using Explore.Cms.Models;

namespace Explore.Cms.Services;

public interface IEventService : IMongoRepository<Events>
{
    Task<Events> UpdateEvent(Events eventItem);
    Task<Events> AddGuestToEvent(Guid eventId, Guid guestId);
    Task<Events> RemoveGuestFromEvent(Guid eventId, Guid guestId);
    Task<IEnumerable<Events>> GetAvailableEvents();
}
