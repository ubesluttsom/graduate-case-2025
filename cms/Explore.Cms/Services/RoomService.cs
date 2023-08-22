using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Explore.Cms.Configuration;
using Explore.Cms.DAL;
using Explore.Cms.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Explore.Cms.Services;

public class RoomService : MongoRepository<Room>, IRoomService
{
    private readonly ILogger<RoomService> _logger;

    public RoomService(IOptions<MongoDbOptions> options, IMongoClient client, ILogger<RoomService> logger) : base(options, client)
    {
        _logger = logger;
    }

    public async Task AddRoom(Room room)
    {
        await AddOneAsync(room);
    }

    public async Task<Room> GetRoom(Guid id)
    {
        return await FindOneByIdAsync(id);
    }

    public async Task<IEnumerable<Room>> GetRooms(Expression<Func<Room, bool>> filterExpression)
    {
        return await FindAsync(filterExpression);
    }

    public async Task<Room> GetNextAvailableRoom()
    {
        var rooms = (await FindAsync(r => true)).ToList();

        if (rooms.Any(r => r.GuestIds.Count == 0)) return rooms.First(r => r.GuestIds.Count == 0);

        var room = new Room
        {
            RoomNumber = !rooms.Any() ? 1 : rooms.Select(r => r.RoomNumber).Max() + 1,
            Id = Guid.NewGuid()
        };

        await AddRoom(room);
        return room;
    }

    public async Task<bool> RemoveGuestFromRoom(Guid roomId, Guid guestId)
    {
        var room = await FindOneByIdAsync(roomId);

        if (room.Id == Guid.Empty) return false;

        room.GuestIds.Remove(guestId);
        await UpdateOneAsync(room);
        return true;
    }

    public async Task<Room> AddTransactionToRoom(Guid roomId, Guid transactionId)
    {
        var room = await GetRoom(roomId);
        room.TransactionIds.Add(transactionId);
        return await UpdateOneAsync(room);
    }

    public async Task<Room> RemoveTransactionFromRoom(Guid roomId, Guid transactionId)
    {
        var room = await FindOneByIdAsync(roomId);
        room.TransactionIds.Remove(transactionId);
        return await UpdateOneAsync(room);
    }

    public async Task<Room> AddGuestToRoom(Guid roomId, Guid guestId)
    {
        var room = await FindOneByIdAsync(roomId);
        room.GuestIds.Add(guestId);
        await UpdateOneAsync(room);
        return room;
    }
}