using Expenses.Application.repositories;
using Expenses.Infrastructure.persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Infrastructure.repositories
{
    public class EfRepository<TEntity, TKey> : IRepository<TEntity, TKey>
        where TEntity : class
    {
        private readonly TransactionsDbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public EfRepository(TransactionsDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.
                Where(entity => EF.Property<int>(entity, "Status") == 0)
                .ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet
                .Where(entity => EF.Property<int>(entity, "Status") == 0)
                .FirstOrDefaultAsync(e => EF.Property<TKey>(e, "Id").Equals(id));
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
