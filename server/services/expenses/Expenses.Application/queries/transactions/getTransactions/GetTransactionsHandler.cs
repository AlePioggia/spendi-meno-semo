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
            return await _repository.GetAllAsync();
        }
    }
}
