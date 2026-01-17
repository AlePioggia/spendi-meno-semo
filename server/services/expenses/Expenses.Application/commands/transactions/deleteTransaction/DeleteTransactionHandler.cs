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
            Transaction transaction = await _repository.GetByIdAsync(command.id) ?? new Transaction();
            if (transaction.Id <= 0)
            {
                return;
            }
            transaction.Delete();
            await _repository.UpdateAsync(transaction);
        }
    }
}
