using System.Threading.Tasks;
using Explore.Cms.DAL;
using Explore.Cms.Models;

namespace Explore.Cms.Services;

public interface IGuestService : IMongoRepository<Guest>
{
    Task<Guest> UpdateGuest(Guest guest);
}