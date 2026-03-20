using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using MediatR;

namespace Expenses.Application.queries.transactions
{
    public record GetTransactionsQuery() : IRequest<List<Transaction>>;

    public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, List<Transaction>?>
    {
        private readonly IRepository<Transaction, long> _repository;
        private readonly ICacheService<List<Transaction>> _cacheService;

        public GetTransactionsHandler(IRepository<Transaction, long> repository, ICacheService<List<Transaction>> cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task<List<Transaction>?> Handle(GetTransactionsQuery query, CancellationToken none)
        {
            return await _cacheService.GetOrCreate(async (x) => await _repository.GetAllAsync());
        }
    }
}
