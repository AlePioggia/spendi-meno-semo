using Expenses.Application.contexts;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.recurringTransactions.createRecurringTransaction
{
    public class CreateRecurringTransactionCommandHandler : IRequestHandler<CreateRecurringTransactionCommand>
    {
        private readonly IRepository<RecurringOperation, long> _repository;
        private readonly IExecutionContext _executionContext;

        public CreateRecurringTransactionCommandHandler(IRepository<RecurringOperation, long> repository,
            IExecutionContext executionContext)
        {
            _repository = repository;
            _executionContext = executionContext;
        }

        public async Task Handle(CreateRecurringTransactionCommand request, CancellationToken cancellationToken)
        {
            TransactionTemplate transactionTemplate = new TransactionTemplate
            {
                Id = 0,
                Description = request.TransactionDescription,
                Amount = new Money(request.Amount),
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
                UserId = _executionContext.UserId,
                TenantId = _executionContext.TenantId
            };

            await _repository.AddAsync(recurringOperation);
        }
    }
}
