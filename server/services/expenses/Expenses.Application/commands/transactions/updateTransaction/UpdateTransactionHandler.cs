using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.updateTransaction
{
    public class UpdateTransactionHandler
    {
        private readonly IRepository<Transaction, long> _repository;

        public UpdateTransactionHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateTransactionCommand command)
        {
            Money money = new Money(command.amount);

            Transaction transaction = new Transaction()
            {
                Id = command.id,
                Description = command.description,
                Amount = money,
                ExpenseType = command.transactionType,
                CategoryId = command.categoryId,
                Date = command.date,
                UserId = command.userId,
                TenantId = command.tenantId
            };

            await _repository.UpdateAsync(transaction);
        }
    }
}
