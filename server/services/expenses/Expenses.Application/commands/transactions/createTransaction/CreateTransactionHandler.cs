using Expenses.Application.repositories;
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

        public CreateTransactionHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateTransactionCommand command, CancellationToken token)
        {
            Money money = new Money(command.Amount, command.Currency);

            Transaction transaction = new Transaction
            {
                Description = command.Description,
                Amount = money,
                ExpenseType = command.ExpenseType,
                UserId = command.UserId,
                TenantId = command.TenantId,
                CategoryId = command.CategoryId,
                Date = command.Date,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(transaction);
        }
    }
}