using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;

namespace Expenses.Application.queries.transactions
{
    public record GetTransactionsQuery() : IRequest<List<Transaction>>;

    public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, List<Transaction>?>
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
