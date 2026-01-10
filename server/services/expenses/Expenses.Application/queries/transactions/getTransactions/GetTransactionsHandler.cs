using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.transactions.getTransactions
{
    public class GetTransactionsHandler: IRequestHandler<GetTransactionsQuery, List<Transaction>?>
    {
        private readonly IRepository<Transaction, long> _repository;

        public GetTransactionsHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task<List<Transaction>?> Handle(GetTransactionsQuery query, CancellationToken none)
        {
            if (query.userId <= 0 || query.tenantId <= 0)
            {
                return null;
            }

            return await _repository.GetAllAsync();
        }
    }
}
