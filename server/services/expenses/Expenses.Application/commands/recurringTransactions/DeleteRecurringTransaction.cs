using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.recurringTransactions
{
    public sealed record DeleteRecurringTransactionCommand(long Id) : IRequest;

    public class DeleteRecurringTransactionHandler : IRequestHandler<DeleteRecurringTransactionCommand>
    {
        private readonly IRepository<RecurringOperation, long> _repository;

        public DeleteRecurringTransactionHandler(IRepository<RecurringOperation, long> repository)
        {
            _repository = repository;
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
