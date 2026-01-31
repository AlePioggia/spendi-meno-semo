using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.categories
{
    public sealed record DeleteCategoryCommand(long id) : IRequest;

    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IRepository<Category, long> _repository;

        public DeleteCategoryHandler(IRepository<Category, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteCategoryCommand command, CancellationToken ct)
        {
            Category? entity = await _repository.GetByIdAsync(command.id);
            if (entity is null)
            {
                return;
            }
            entity.Delete();
            await _repository.UpdateAsync(entity);
        }
    }

    public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0)
                .WithMessage("CategoryId is required.");
        }
    }
}
