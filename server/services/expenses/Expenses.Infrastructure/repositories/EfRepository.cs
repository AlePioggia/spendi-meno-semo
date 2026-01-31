using Expenses.Application.repositories;
using Expenses.Infrastructure.persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var idProperty = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(TKey) },
                parameter,
                Expression.Constant("Id"));

            var equals = Expression.Equal(idProperty, Expression.Constant(id, typeof(TKey)));
            var predicate = Expression.Lambda<Func<TEntity, bool>>(equals, parameter);

            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
