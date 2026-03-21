using Expenses.Application.contexts;
using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.transactions
{
    public sealed record CreateTransactionCommand(
        string? Description,
        decimal Amount,
        Currency Currency,
        TransactionType ExpenseType,
        long CategoryId,
        DateTime Date,
        bool isProxyTransaction = false
    ) : IRequest;

    public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand>
    {
        private readonly IRepository<Transaction, long> _repository;
        private readonly IExecutionContext _executionContext;
        private readonly ICacheService<Transaction> _cacheService;

        public CreateTransactionHandler(
            IRepository<Transaction, long> repository,
            IExecutionContext executionContext,
            ICacheService<Transaction> cacheService)
        {
            _repository = repository;
            _executionContext = executionContext;
            _cacheService = cacheService;
        }

        public async Task Handle(CreateTransactionCommand command, CancellationToken token)
        {
            Money money = new Money(command.Amount, command.Currency);

            Transaction transaction = new Transaction
            {
                Description = command.Description,
                Amount = money,
                ExpenseType = command.ExpenseType,
                UserId = _executionContext.UserId,
                TenantId = _executionContext.TenantId,
                CategoryId = command.CategoryId,
                Date = command.Date,
                IsProxyTransaction = command.isProxyTransaction,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(transaction);
            await _cacheService.Invalidate();
        }
    }

    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("CategoryId must be greater than zero.");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required.");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
