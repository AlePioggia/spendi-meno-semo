using Expenses.Infrastructure.persistence;
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
            //var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION");
            var connectionString = "Server=localhost,1433;Database=TransactionsDb;User Id=sa;Password=StrongPassw0rd!;TrustServerCertificate=True;MultipleActiveResultSets=True";

            var optionsBuilder = new DbContextOptionsBuilder<TransactionsDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new TransactionsDbContext(optionsBuilder.Options);
        }
    }
}