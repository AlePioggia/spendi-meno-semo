using Expenses.Domain.Entities;
using Expenses.Application.contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace Expenses.Infrastructure.persistence
{
    public class TransactionsDbContext: DbContext
    {
        private readonly IExecutionContext _executionContext;

        public long CurrentTenantId => _executionContext.TenantId;
        public long CurrentUserId => _executionContext.UserId;

        public TransactionsDbContext(
            DbContextOptions<TransactionsDbContext> options,
            IExecutionContext executionContext)
            : base(options)
        {
            _executionContext = executionContext;
        }

        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(TransactionsDbContext).Assembly);

            modelBuilder.Entity<Transaction>()
                .HasQueryFilter(t => t.Status == 0 && t.TenantId == CurrentTenantId && t.UserId == CurrentUserId);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(c => c.Status == 0 && c.TenantId == CurrentTenantId && c.UserId == CurrentUserId);
        }
    }
}
