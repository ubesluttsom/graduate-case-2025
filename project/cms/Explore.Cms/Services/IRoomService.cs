using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Explore.Cms.DAL;
using Explore.Cms.Models;
using MongoDB.Bson;

namespace Explore.Cms.Services;

public interface IRoomService : IMongoRepository<Room>
{
    Task AddRoom(Room room);
    Task<Room> GetRoom(ObjectId id);
    Task<IEnumerable<Room>> GetRooms(Expression<Func<Room, bool>> filterExpression);
    Task<bool> RemoveGuestFromRoom(ObjectId roomId, ObjectId guestId);
    Task<Room> AddTransactionToRoom(ObjectId roomId, ObjectId transactionId);
    Task<Room> AddGuestToRoom(ObjectId roomId, ObjectId guestId);
    Task<Room> GetNextAvailableRoom();
}