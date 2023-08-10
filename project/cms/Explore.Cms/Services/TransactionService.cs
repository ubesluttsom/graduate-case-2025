using Explore.Cms.Configuration;
using Explore.Cms.DAL;
using Explore.Cms.Models;
using Microsoft.Extensions.Options;

namespace Explore.Cms.Services;

public class TransactionService : MongoRepository<GuestTransaction>, ITransactionService
{
    public TransactionService(IOptions<MongoDbOptions> options) : base(options)
    {
    }
}