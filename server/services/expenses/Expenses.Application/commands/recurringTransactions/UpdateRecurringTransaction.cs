using Expenses.Application.contexts;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.recurringTransactions
{
    public sealed record UpdateRecurringTransactionCommand(
        long Id,
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

    public class UpdateRecurringTransactionHandler : IRequestHandler<UpdateRecurringTransactionCommand>
    {
        private readonly IRepository<RecurringOperation, long> _repository;
        private readonly IExecutionContext _executionContext;

        public UpdateRecurringTransactionHandler(
            IRepository<RecurringOperation, long> repository,
            IExecutionContext executionContext)
        {
            _repository = repository;
            _executionContext = executionContext;
        }

        public async Task Handle(UpdateRecurringTransactionCommand command, CancellationToken ct)
        {
            RecurringOperation? recurringOperation = await _repository.GetByIdAsync(command.Id);
            if (recurringOperation is null)
            {
                return;
            }

            recurringOperation.Description = command.RecurringDescription;
            recurringOperation.Frequency = command.RecurringOperationFrequency;
            recurringOperation.StartDate = command.StartDate;
            recurringOperation.EndDate = command.EndDate;
            recurringOperation.CategoryId = command.CategoryId;

            if (recurringOperation.Template is null)
            {
                recurringOperation.Template = new TransactionTemplate
                {
                    Id = recurringOperation.TemplateId,
                    CreatedAt = DateTime.Now,
                    UserId = _executionContext.UserId,
                    TenantId = _executionContext.TenantId
                };
            }

            recurringOperation.Template.Description = command.TransactionDescription;
            recurringOperation.Template.Amount = new Money(command.Amount, command.Currency);
            recurringOperation.Template.TransactionType = command.TransactionType;
            recurringOperation.Template.CategoryId = command.CategoryId;
            recurringOperation.Template.Date = command.TransactionDate;
            recurringOperation.Template.UserId = recurringOperation.UserId;
            recurringOperation.Template.TenantId = recurringOperation.TenantId;

            await _repository.UpdateAsync(recurringOperation);
        }
    }

    public class UpdateRecurringTransactionCommandValidator : AbstractValidator<UpdateRecurringTransactionCommand>
    {
        public UpdateRecurringTransactionCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

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
