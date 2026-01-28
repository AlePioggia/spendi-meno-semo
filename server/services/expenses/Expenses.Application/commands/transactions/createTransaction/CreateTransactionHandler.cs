using Expenses.Application.repositories;
using Expenses.Application.contexts;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.createTransaction
{
    public class CreateTransactionHandler: IRequestHandler<CreateTransactionCommand>
    {
        private readonly IRepository<Transaction, long> _repository;
        private readonly IExecutionContext _executionContext;

        public CreateTransactionHandler(IRepository<Transaction, long> repository, IExecutionContext executionContext)
        {
            _repository = repository;
            _executionContext = executionContext;
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
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(transaction);
        }
    }
}