using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.recurringTransactions
{
    public sealed record DeleteRecurringTransactionCommand(long Id) : IRequest;

    public class DeleteRecurringTransactionHandler : IRequestHandler<DeleteRecurringTransactionCommand>
    {
        private readonly IRepository<RecurringOperation, long> _repository;
        private readonly ICacheService<RecurringOperation> _cacheService;

        public DeleteRecurringTransactionHandler(IRepository<RecurringOperation, long> repository, ICacheService<RecurringOperation> cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteRecurringTransactionCommand command, CancellationToken ct)
        {
            RecurringOperation? recurringOperation = await _repository.GetByIdAsync(command.Id);
            if (recurringOperation is null)
            {
                return;
            }

            recurringOperation.Delete();
            await _repository.UpdateAsync(recurringOperation);
            await _cacheService.Invalidate();
        }
    }

    public class DeleteRecurringTransactionCommandValidator : AbstractValidator<DeleteRecurringTransactionCommand>
    {
        public DeleteRecurringTransactionCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
