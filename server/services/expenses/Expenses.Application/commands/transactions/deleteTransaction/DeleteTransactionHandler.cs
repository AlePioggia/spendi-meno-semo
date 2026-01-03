using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.deleteTransaction
{
    public class DeleteTransactionHandler
    {
        private readonly IRepository<Transaction, long> _repository;

        public DeleteTransactionHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteTransactionCommand command)
        {
            var transaction = new Transaction
            {
                Id = command.id,
                UserId = command.userId,
                TenantId = command.tenantId
            };

            await _repository.DeleteAsync(transaction);
        }
    }
}
