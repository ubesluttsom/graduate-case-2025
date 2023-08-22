using Explore.Cms.Configuration;
using Explore.Cms.DAL;
using Explore.Cms.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Explore.Cms.Services;

public class TransactionService : MongoRepository<GuestTransaction>, ITransactionService
{
    public TransactionService(IOptions<MongoDbOptions> options, IMongoClient client) : base(options, client)
    {
    }
}