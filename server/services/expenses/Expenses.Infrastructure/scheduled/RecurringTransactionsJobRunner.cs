using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Infrastructure.repositories;

namespace Expenses.Infrastructure.scheduled
{
    public sealed class RecurringTransactionsJobRunner
    {
        private readonly RecurringOperationRepository _recurringOperationRepository;
        private readonly IRepository<Transaction, long> _transactionRepository;

        public RecurringTransactionsJobRunner(
            RecurringOperationRepository recurringOperationRepository,
            IRepository<Transaction, long> transactionRepository)
        {
            _recurringOperationRepository = recurringOperationRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task RunOnceAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;

            var recurringOperations = await _recurringOperationRepository.GetAllAsyncWithoutQueryFilters();
            var frequencies = recurringOperations?
                .Where(x => x.StartDate <= now && x.EndDate >= now)
                .Select(x => x.Frequency)
                .Distinct()
                .ToList();

            if (frequencies == null || frequencies.Count == 0)
                return;

            foreach (var frequency in frequencies)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var eligibleOperations = await _recurringOperationRepository.GetElegibleOperationsByFrequency(frequency);
                await InsertTransactionsByFrequency(eligibleOperations, frequency);
            }
        }

        private async Task InsertTransactionsByFrequency(List<RecurringOperation> recurrentOperations, RecurringOperationFrequency frequency)
        {
            switch (frequency)
            {
                case RecurringOperationFrequency.Daily:
                    await InsertDailyRecurringOperation(recurrentOperations);
                    break;
                case RecurringOperationFrequency.Weekly:
                    await InsertWeeklyRecurringOperation(recurrentOperations);
                    break;
                case RecurringOperationFrequency.Monthly:
                    await InsertMonthlyRecurringOperation(recurrentOperations);
                    break;
                case RecurringOperationFrequency.Yearly:
                    await InsertYearlyRecurringOperation(recurrentOperations);
                    break;
                default:
                    break;
            }
        }

        private async Task InsertDailyRecurringOperation(List<RecurringOperation> recurringOperations)
        {
            var now = DateTime.Now;
            var start = now.Date;
            var end = start.AddDays(1);

            foreach (var x in recurringOperations)
            {
                var alreadyInsertedToday = x.Transactions.Any(y =>
                    y.RecurringOperationId == x.Id &&
                    y.Date >= start && y.Date < end);

                if (alreadyInsertedToday)
                    continue;

                await _transactionRepository.AddAsync(new Transaction
                {
                    Description = x.Template.Description,
                    Amount = x.Template.Amount,
                    ExpenseType = x.Template.TransactionType,
                    CategoryId = x.Template.CategoryId,
                    Date = now,
                    RecurringOperationId = x.Id,
                    UserId = x.UserId,
                    TenantId = x.TenantId
                });
            }
        }

        private async Task InsertWeeklyRecurringOperation(List<RecurringOperation> recurringOperations)
        {
            var now = DateTime.Now;
            var weekStart = now.Date.AddDays(-(((int)now.DayOfWeek + 6) % 7));
            var weekEnd = weekStart.AddDays(7);

            foreach (var x in recurringOperations)
            {
                var alreadyInsertedThisWeek = x.Transactions.Any(y =>
                    y.RecurringOperationId == x.Id &&
                    y.Date >= weekStart && y.Date < weekEnd);

                if (alreadyInsertedThisWeek)
                    continue;

                var day = Math.Min(x.Template.Date.Day, DateTime.DaysInMonth(now.Year, now.Month));
                var insertDate = new DateTime(now.Year, now.Month, day, 0, 0, 0, DateTimeKind.Local);

                await _transactionRepository.AddAsync(new Transaction
                {
                    Description = x.Template.Description,
                    Amount = x.Template.Amount,
                    ExpenseType = x.Template.TransactionType,
                    CategoryId = x.Template.CategoryId,
                    Date = insertDate,
                    RecurringOperationId = x.Id,
                    UserId = x.UserId,
                    TenantId = x.TenantId
                });
            }
        }

        private async Task InsertMonthlyRecurringOperation(List<RecurringOperation> recurringOperations)
        {
            var now = DateTime.Now;

            foreach (var x in recurringOperations)
            {
                var alreadyInsertedThisMonth = x.Transactions.Any(y =>
                    y.RecurringOperationId == x.Id &&
                    y.Date.Year == now.Year &&
                    y.Date.Month == now.Month);

                if (alreadyInsertedThisMonth)
                    continue;

                var day = Math.Min(x.Template.Date.Day, DateTime.DaysInMonth(now.Year, now.Month));
                var insertDate = new DateTime(now.Year, now.Month, day, 0, 0, 0, DateTimeKind.Local);

                await _transactionRepository.AddAsync(new Transaction
                {
                    Description = x.Template.Description,
                    Amount = x.Template.Amount,
                    ExpenseType = x.Template.TransactionType,
                    CategoryId = x.Template.CategoryId,
                    Date = insertDate,
                    RecurringOperationId = x.Id,
                    UserId = x.UserId,
                    TenantId = x.TenantId
                });
            }
        }

        private async Task InsertYearlyRecurringOperation(List<RecurringOperation> recurringOperations)
        {
            var now = DateTime.Now;

            foreach (var x in recurringOperations)
            {
                var alreadyInsertedThisYear = x.Transactions.Any(y =>
                    y.RecurringOperationId == x.Id &&
                    y.Date.Year == now.Year);

                if (alreadyInsertedThisYear)
                    continue;

                var month = x.Template.Date.Month;
                var day = Math.Min(x.Template.Date.Day, DateTime.DaysInMonth(now.Year, month));
                var insertDate = new DateTime(now.Year, month, day, 0, 0, 0, DateTimeKind.Local);

                await _transactionRepository.AddAsync(new Transaction
                {
                    Description = x.Template.Description,
                    Amount = x.Template.Amount,
                    ExpenseType = x.Template.TransactionType,
                    CategoryId = x.Template.CategoryId,
                    Date = insertDate,
                    RecurringOperationId = x.Id,
                    TenantId = x.TenantId,
                    UserId = x.UserId
                });
            }
        }
    }
}
