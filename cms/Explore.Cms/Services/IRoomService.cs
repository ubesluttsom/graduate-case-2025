using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Explore.Cms.DAL;
using Explore.Cms.Models;

namespace Explore.Cms.Services;

public interface IRoomService : IMongoRepository<Room>
{
    Task AddRoom(Room room);
    Task<Room> GetRoom(Guid id);
    Task<IEnumerable<Room>> GetRooms(Expression<Func<Room, bool>> filterExpression);
    Task<bool> RemoveGuestFromRoom(Guid roomId, Guid guestId);
    Task<Room> AddTransactionToRoom(Guid roomId, Guid transactionId);
    Task<Room> AddGuestToRoom(Guid roomId, Guid guestId);
    Task<Room> GetNextAvailableRoom();
}