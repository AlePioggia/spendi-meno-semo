using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.deleteTransaction
{
    public class DeleteTransactionHandler: IRequestHandler<DeleteTransactionCommand>
    {
        private readonly IRepository<Transaction, long> _repository;

        public DeleteTransactionHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteTransactionCommand command, CancellationToken ct)
        {
            Transaction? transaction = await _repository.GetByIdAsync(command.id);
            if (transaction is null)
            {
                return;
            }
            transaction.Delete();
            await _repository.UpdateAsync(transaction);
        }
    }
}
