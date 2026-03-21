using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.categories
{
    public sealed record DeleteCategoryCommand(long id) : IRequest;

    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IRepository<Category, long> _repository;
        private readonly ICacheService<Category> _cacheService;

        public DeleteCategoryHandler(IRepository<Category, long> repository, ICacheService<Category> cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
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
            await _cacheService.Invalidate();
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
