using Expenses.Application.contexts;
using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.recurringTransactions
{
    public sealed record CreateRecurringTransactionCommand(
        string RecurringDescription,
        string TransactionDescription,
        Currency Currency,
        decimal Amount,
        TransactionType TransactionType,
        long CategoryId,
        RecurringOperationFrequency RecurringOperationFrequency,
        DateTime StartDate,
        DateTime EndDate,
        DateTime TransactionDate
    ) : IRequest;

    public class CreateRecurringTransactionCommandHandler : IRequestHandler<CreateRecurringTransactionCommand>
    {
        private readonly IRepository<RecurringOperation, long> _repository;
        private readonly IExecutionContext _executionContext;
        private readonly ICacheService<RecurringOperation> _cacheService;

        public CreateRecurringTransactionCommandHandler(
            IRepository<RecurringOperation, long> repository,
            IExecutionContext executionContext,
            ICacheService<RecurringOperation> cacheService)
        {
            _repository = repository;
            _executionContext = executionContext;
            _cacheService = cacheService;
        }

        public async Task Handle(CreateRecurringTransactionCommand request, CancellationToken cancellationToken)
        {
            TransactionTemplate transactionTemplate = new TransactionTemplate
            {
                Id = 0,
                Description = request.TransactionDescription,
                Amount = new Money(request.Amount, request.Currency),
                TransactionType = request.TransactionType,
                CategoryId = request.CategoryId,
                Date = request.TransactionDate,
                CreatedAt = DateTime.Now,
                UserId = _executionContext.UserId,
                TenantId = _executionContext.TenantId
            };

            RecurringOperation recurringOperation = new RecurringOperation
            {
                Id = 0,
                Description = request.RecurringDescription,
                Frequency = request.RecurringOperationFrequency,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Template = transactionTemplate,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.Now,
                UserId = _executionContext.UserId,
                TenantId = _executionContext.TenantId
            };

            await _repository.AddAsync(recurringOperation);
            await _cacheService.Invalidate();
        }
    }

    public class CreateRecurringTransactionCommandValidator : AbstractValidator<CreateRecurringTransactionCommand>
    {
        public CreateRecurringTransactionCommandValidator()
        {
            RuleFor(x => x.RecurringDescription)
                .NotEmpty();

            RuleFor(x => x.TransactionDescription)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0);

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate);
        }
    }
}
