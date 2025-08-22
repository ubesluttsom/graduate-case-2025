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
    private readonly ITransactionService _transactionService;
    private readonly IGuestService _guestService;
    private readonly IRoomService _roomService;

    public EventService(IOptions<MongoDbOptions> options, IMongoClient client, ILogger<EventService> logger, 
        ITransactionService transactionService, IGuestService guestService, IRoomService roomService) 
        : base(options, client)
    {
        _logger = logger;
        _transactionService = transactionService;
        _guestService = guestService;
        _roomService = roomService;
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

        // Add transaction to room tab
        var guest = await _guestService.FindOneByIdAsync(guestId);
        if (guest.Id != Guid.Empty && guest.RoomId != Guid.Empty)
        {
            var transaction = new GuestTransaction
            {
                Amount = eventItem.Price,
                Description = $"Event booking: {eventItem.Name}",
                RoomId = guest.RoomId,
                GuestId = guestId
            };

            await _transactionService.AddOneAsync(transaction);
            await _roomService.AddTransactionToRoom(guest.RoomId, transaction.Id);
        }

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
