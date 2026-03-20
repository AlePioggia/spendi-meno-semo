using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using MediatR;

namespace Expenses.Application.queries.recurringTransactions
{
    public sealed record GetRecurringTransactionsQuery() : IRequest<List<RecurringOperation>?>;

    public class GetRecurringTransactionsHandler : IRequestHandler<GetRecurringTransactionsQuery, List<RecurringOperation>?>
    {
        private readonly IRepository<RecurringOperation, long> _repository;
        private readonly ICacheService<List<RecurringOperation>> _cacheService;

        public GetRecurringTransactionsHandler(IRepository<RecurringOperation, long> repository, ICacheService<List<RecurringOperation>> cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task<List<RecurringOperation>?> Handle(GetRecurringTransactionsQuery query, CancellationToken ct)
        {
            return await _repository.GetAllAsync();
        }
    }
}
