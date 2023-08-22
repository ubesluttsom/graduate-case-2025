using System.Linq.Expressions;
using Explore.Cms.Configuration;
using Explore.Cms.Models;
using Explore.Cms.Services;
using Explore.Cms.Test.TestUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;

namespace Explore.Cms.Test.MongoRepository;

public class TransactionServiceTests : MongoRepositoryTests<GuestTransaction>
{
    public TransactionServiceTests()
    {
    }
}