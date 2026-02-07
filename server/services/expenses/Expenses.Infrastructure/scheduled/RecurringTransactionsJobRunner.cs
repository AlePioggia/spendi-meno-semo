using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Infrastructure.repositories;
using System;

namespace Expenses.Infrastructure.scheduled
{
    public sealed class RecurringTransactionsJobRunner
    {
        private readonly RecurringOperationRepository _recurringOperationRepository;
        private readonly IRepository<Transaction, long> _transactionRepository;
        private List<RecurringOperation> _recurringOperations;

        public RecurringTransactionsJobRunner(
            RecurringOperationRepository recurringOperationRepository,
            IRepository<Transaction, long> transactionRepository)
        {
            _recurringOperationRepository = recurringOperationRepository;
            _transactionRepository = transactionRepository;
            _recurringOperations = new List<RecurringOperation>();
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
                _recurringOperations = await _recurringOperationRepository.GetElegibleOperationsByFrequency(frequency);
                await InsertTransactionsByFrequency(frequency);
            }
        }

        private async Task InsertTransactionsByFrequency(RecurringOperationFrequency frequency)
        {
            await (frequency switch
            {
                RecurringOperationFrequency.Daily => InsertDailyRecurringOperation(),
                RecurringOperationFrequency.Weekly => InsertWeeklyRecurringOperation(),
                RecurringOperationFrequency.Monthly => InsertMonthlyRecurringOperation(),
                RecurringOperationFrequency.Yearly => InsertYearlyRecurringOperation(),
                _ => Task.CompletedTask
            });
        }

        private async Task InsertDailyRecurringOperation()
        {
            await InsertRecurringOperation(
                x => x.Transactions.Any(y =>
                    y.RecurringOperationId == x.Id &&
                    y.Date >= DateTime.Now.Date && y.Date < DateTime.Now.Date.AddDays(1)),
                _ => DateTime.Now
            );
        }

        private async Task InsertWeeklyRecurringOperation()
        {
            var now = DateTime.Now;
            var weekStart = now.Date.AddDays(-(((int)now.DayOfWeek + 6) % 7));
            var weekEnd = weekStart.AddDays(7);

            await InsertRecurringOperation(
                x => x.Transactions.Any(y =>
                    y.RecurringOperationId == x.Id &&
                    y.Date >= weekStart && y.Date < weekEnd),
                x =>
                {
                    var day = Math.Min(
                       x.Template.Date.Day,
                       DateTime.DaysInMonth(now.Year, now.Month));

                    return new DateTime(now.Year, now.Month, day, 0, 0, 0, DateTimeKind.Local);
                }
            );
        }

        private async Task InsertMonthlyRecurringOperation()
        {
            await InsertRecurringOperation(
                x => x.Transactions.Any(y =>
                    y.RecurringOperationId == x.Id &&
                    y.Date.Year == DateTime.Now.Year &&
                    y.Date.Month == DateTime.Now.Month),
                x =>
                {
                    var day = Math.Min(
                        x.Template.Date.Day,
                        DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    return new DateTime(DateTime.Now.Year, DateTime.Now.Month, day, 0, 0, 0, DateTimeKind.Local);
                }
            );
        }

        private async Task InsertYearlyRecurringOperation()
        {
            await InsertRecurringOperation(
                x => x.Transactions.Any(y =>
                    y.RecurringOperationId == x.Id &&
                    y.Date.Year == DateTime.Now.Year),
                x =>
                {
                    var month = x.Template.Date.Month;
                    var day = Math.Min(x.Template.Date.Day, DateTime.DaysInMonth(DateTime.Now.Year, month));
                    return new DateTime(DateTime.Now.Year, month, day, 0, 0, 0, DateTimeKind.Local);
                }
            );
        }

        private async Task InsertRecurringOperation(Predicate<RecurringOperation> predicate, Func<RecurringOperation, DateTime> dateFactory)
        {
            foreach (var x in _recurringOperations)
            {
                if (predicate(x)) continue;
                await _transactionRepository.AddAsync(new Transaction
                {
                    Description = x.Template.Description,
                    Amount = x.Template.Amount,
                    ExpenseType = x.Template.TransactionType,
                    CategoryId = x.Template.CategoryId,
                    Date = dateFactory(x),
                    RecurringOperationId = x.Id,
                    TenantId = x.TenantId,
                    UserId = x.UserId
                }); 
            }
        }
    }
}
