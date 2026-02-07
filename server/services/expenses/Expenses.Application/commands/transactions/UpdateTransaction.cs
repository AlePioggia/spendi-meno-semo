using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.transactions
{
    public record UpdateTransactionCommand(
        long id,
        string description,
        decimal amount,
        TransactionType transactionType,
        Currency currency,
        long categoryId,
        DateTime date
    ) : IRequest;

    public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionCommand>
    {
        private readonly IRepository<Transaction, long> _repository;

        public UpdateTransactionHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateTransactionCommand command, CancellationToken cancellationToken)
        {
            Money money = new Money(command.amount);

            Transaction? transaction = await _repository.GetByIdAsync(command.id);
            if (transaction is null)
            {
                return;
            }

            transaction.Description = command.description;
            transaction.Amount = money;
            transaction.ExpenseType = command.transactionType;
            transaction.CategoryId = command.categoryId;
            transaction.Date = command.date;

            await _repository.UpdateAsync(transaction);
        }
    }

    public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommand>
    {
        public UpdateTransactionCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0).WithMessage("Transaction Id must be greater than zero.");
            RuleFor(x => x.date)
                .NotNull().WithMessage("Date must be provided.");
            RuleFor(x => x.categoryId)
                .GreaterThan(0).WithMessage("Category Id must be greater than zero.");
            RuleFor(x => x.amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
