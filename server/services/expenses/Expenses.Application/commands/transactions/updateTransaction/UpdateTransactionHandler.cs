using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.updateTransaction
{
    public class UpdateTransactionHandler: IRequestHandler<UpdateTransactionCommand>
    {
        private readonly IRepository<Transaction, long> _repository;

        public UpdateTransactionHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateTransactionCommand command, CancellationToken cancellationToken)
        {
            Money money = new Money(command.amount);

            Transaction? transaction = await _repository.GetByIdAsync(command.id);
            if (transaction is null)
            {
                return;
            }

            transaction.Description = command.description;
            transaction.Amount = money;
            transaction.ExpenseType = command.transactionType;
            transaction.CategoryId = command.categoryId;
            transaction.Date = command.date;

            await _repository.UpdateAsync(transaction);
        }
    }
}
