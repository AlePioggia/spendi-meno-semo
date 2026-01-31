using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.queries.recurringTransactions
{
    public sealed record GetRecurringTransactionByIdQuery(long Id) : IRequest<RecurringOperation?>;

    public class GetRecurringTransactionByIdHandler : IRequestHandler<GetRecurringTransactionByIdQuery, RecurringOperation?>
    {
        private readonly IRepository<RecurringOperation, long> _repository;

        public GetRecurringTransactionByIdHandler(IRepository<RecurringOperation, long> repository)
        {
            _repository = repository;
        }

        public async Task<RecurringOperation?> Handle(GetRecurringTransactionByIdQuery query, CancellationToken ct)
        {
            return await _repository.GetByIdAsync(query.Id);
        }
    }

    public class GetRecurringTransactionByIdQueryValidator : AbstractValidator<GetRecurringTransactionByIdQuery>
    {
        public GetRecurringTransactionByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
