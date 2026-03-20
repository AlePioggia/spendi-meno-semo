using Expenses.Application.repositories;
using Expenses.Application.services;
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
        DateTime date,
        bool isProxyTransaction = false
    ) : IRequest;

    public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionCommand>
    {
        private readonly IRepository<Transaction, long> _repository;
        private readonly ICacheService<Transaction> _cacheService;

        public UpdateTransactionHandler(IRepository<Transaction, long> repository, ICacheService<Transaction> cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
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
            transaction.IsProxyTransaction = command.isProxyTransaction;

            await _repository.UpdateAsync(transaction);
            await _cacheService.Invalidate();
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
