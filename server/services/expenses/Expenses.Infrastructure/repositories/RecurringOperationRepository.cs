using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Infrastructure.persistence;
using Microsoft.EntityFrameworkCore;

namespace Expenses.Infrastructure.repositories
{
    public class RecurringOperationRepository : IRepository<RecurringOperation, long>
    {
        private readonly TransactionsDbContext _dbContext;

        public RecurringOperationRepository(TransactionsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RecurringOperation?> GetByIdAsync(long id)
        {
            return await _dbContext.Set<RecurringOperation>()
                .Include(x => x.Template)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<RecurringOperation>> GetAllAsync()
        {
            return await _dbContext.Set<RecurringOperation>()
                .Include(x => x.Template)
                .Include(x => x.Category)
                .ToListAsync();
        }

        public async Task<List<RecurringOperation>> GetAllAsyncWithoutQueryFilters()
        {
            return await _dbContext.Set<RecurringOperation>()
                .IgnoreQueryFilters()
                .Include(x => x.Template)
                .Where(x => x.Template.Status == 0)
                .Include(x => x.Category)
                .Where(x => x.Category.Status == 0)
                .ToListAsync();
        }

        public async Task AddAsync(RecurringOperation entity)
        {
            _dbContext.Set<RecurringOperation>().Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(RecurringOperation entity)
        {
            _dbContext.Set<RecurringOperation>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(RecurringOperation entity)
        {
            _dbContext.Set<RecurringOperation>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<RecurringOperation>> GetElegibleOperationsByFrequency(RecurringOperationFrequency frequency)
        {
            return await _dbContext.Set<RecurringOperation>()
                .Include(x => x.Template)
                .Include(x => x.Category)
                .Include(x => x.Transactions)
                .IgnoreQueryFilters()
                .Where(x => x.Frequency == frequency)
                .Where(x => x.Template.Status == 0)
                .Where(x => x.Category.Status == 0)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
