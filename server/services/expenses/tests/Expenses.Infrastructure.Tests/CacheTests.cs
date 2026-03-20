using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using Expenses.Infrastructure.repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using System.Linq.Expressions;

namespace Expenses.Infrastructure.Tests
{
    public class CacheTests
    {
        [Fact]
        public void TestCacheHits()
        {
            var mockDbSet = new Mock<DbSet<Transaction>>();
        }
    }
}
