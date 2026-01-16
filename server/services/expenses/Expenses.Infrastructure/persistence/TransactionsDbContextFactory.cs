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
            var optionsBuilder = new DbContextOptionsBuilder<TransactionsDbContext>();

            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TransactionsDb;Trusted_Connection=True;");

            return new TransactionsDbContext(optionsBuilder.Options);
        }
    }
}