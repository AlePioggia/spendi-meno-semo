using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;

namespace Expenses.Application.queries.recurringTransactions
{
    public sealed record GetRecurringTransactionsQuery() : IRequest<List<RecurringOperation>?>;

    public class GetRecurringTransactionsHandler : IRequestHandler<GetRecurringTransactionsQuery, List<RecurringOperation>?>
    {
        private readonly IRepository<RecurringOperation, long> _repository;

        public GetRecurringTransactionsHandler(IRepository<RecurringOperation, long> repository)
        {
            _repository = repository;
        }

        public async Task<List<RecurringOperation>?> Handle(GetRecurringTransactionsQuery query, CancellationToken ct)
        {
            return await _repository.GetAllAsync();
        }
    }
}
