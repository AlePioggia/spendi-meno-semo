using Expenses.Infrastructure.persistence;
using Expenses.Application.contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Infrastructure.Persistence
{
    public class TransactionsDbContextFactory : IDesignTimeDbContextFactory<TransactionsDbContext>
    {
        public TransactionsDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("LOCAL_CONNECTION_STRING");
            
            var optionsBuilder = new DbContextOptionsBuilder<TransactionsDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            IExecutionContext executionContext = new Expenses.Application.contexts.ExecutionContext
            {
                TenantId = 1,
                UserId = 1
            };

            return new TransactionsDbContext(optionsBuilder.Options, executionContext);
        }
    }
}