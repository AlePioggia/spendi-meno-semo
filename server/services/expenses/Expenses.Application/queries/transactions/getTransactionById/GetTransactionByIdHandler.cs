using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.transactions.getTransactionById
{
    public class GetTransactionByIdHandler: IRequestHandler<GetTransactionByIdQuery, Transaction?>
    {
        private readonly IRepository<Transaction, long> _repository;

        public GetTransactionByIdHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task<Transaction?> Handle(
            GetTransactionByIdQuery query,
            CancellationToken ct
        )
        {
            Transaction? transaction = await _repository.GetByIdAsync(query.TransactionId);

            if (transaction?.TenantId != query.TenantId || transaction.UserId != query.UserId)
            {
                return null;
            }

            return transaction;
        }
    }
}
