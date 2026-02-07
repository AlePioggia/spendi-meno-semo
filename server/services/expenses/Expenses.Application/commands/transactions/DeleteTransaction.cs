using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.transactions
{
    public record DeleteTransactionCommand(long id) : IRequest;

    public class DeleteTransactionHandler : IRequestHandler<DeleteTransactionCommand>
    {
        private readonly IRepository<Transaction, long> _repository;

        public DeleteTransactionHandler(IRepository<Transaction, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteTransactionCommand command, CancellationToken ct)
        {
            Transaction? transaction = await _repository.GetByIdAsync(command.id);
            if (transaction is null)
            {
                return;
            }
            transaction.Delete();
            await _repository.UpdateAsync(transaction);
        }
    }

    public class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommand>
    {
        public DeleteTransactionCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0).WithMessage("Transaction Id must be greater than zero.");
        }
    }
}
