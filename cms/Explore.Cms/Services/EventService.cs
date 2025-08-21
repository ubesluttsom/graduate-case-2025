using System.Collections.Generic;
using System.Threading.Tasks;
using Explore.Cms.Configuration;
using Explore.Cms.DAL;
using Explore.Cms.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Explore.Cms.Services;

public class EventService : MongoRepository<Events>, IEventService
{
    private readonly ILogger<EventService> _logger;

    public EventService(IOptions<MongoDbOptions> options, IMongoClient client, ILogger<EventService> logger) 
        : base(options, client)
    {
        _logger = logger;
    }

    public async Task<Events> UpdateEvent(Events eventItem)
    {
        if (eventItem.Id == Guid.Empty) return eventItem;
        return await UpdateOneAsync(eventItem);
    }

    public async Task<Events> AddGuestToEvent(Guid eventId, Guid guestId)
    {
        var eventItem = await FindOneByIdAsync(eventId);
        if (eventItem.Id == Guid.Empty) return eventItem;

        if (eventItem.GuestIds.Contains(guestId)) return eventItem;
        
        if (eventItem.GuestIds.Count >= eventItem.AvailableSpots)
            throw new InvalidOperationException("Event is full");

        eventItem.GuestIds.Add(guestId);
        return await UpdateOneAsync(eventItem);
    }

    public async Task<Events> RemoveGuestFromEvent(Guid eventId, Guid guestId)
    {
        var eventItem = await FindOneByIdAsync(eventId);
        if (eventItem.Id == Guid.Empty) return eventItem;

        eventItem.GuestIds.Remove(guestId);
        return await UpdateOneAsync(eventItem);
    }

    public async Task<IEnumerable<Events>> GetAvailableEvents()
    {
        return await FindAsync(e => e.AvailableSpots > e.GuestIds.Count && e.Date > DateTime.Now);
    }
}
