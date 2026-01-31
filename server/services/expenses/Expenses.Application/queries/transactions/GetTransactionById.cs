using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.queries.transactions
{
    public record GetTransactionByIdQuery(long TransactionId) : IRequest<Transaction?>;

    public class GetTransactionByIdHandler : IRequestHandler<GetTransactionByIdQuery, Transaction?>
    {
        private readonly IRepository<Transaction, long> _repository;

        public GetTransactionByIdHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task<Transaction?> Handle(GetTransactionByIdQuery query, CancellationToken ct)
        {
            return await _repository.GetByIdAsync(query.TransactionId);
        }
    }

    public class GetTransactionByIdQueryValidator : AbstractValidator<GetTransactionByIdQuery>
    {
        public GetTransactionByIdQueryValidator()
        {
            RuleFor(x => x.TransactionId)
                .GreaterThan(0).WithMessage("TransactionId must be greater than 0.");
        }
    }
}
