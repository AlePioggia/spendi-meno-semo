using Expenses.Application.contexts;
using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.commands.categories
{
    public sealed record CreateCategoryCommand(
        string name,
        string description,
        DateTime createdAt
    ) : IRequest;

    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand>
    {
        private readonly IRepository<Category, long> _repository;
        private readonly IExecutionContext _executionContext;
        private readonly ICacheService<Category> _cacheService;

        public CreateCategoryHandler(
            IRepository<Category, long> repository,
            IExecutionContext executionContext,
            ICacheService<Category> cacheService)
        {
            _repository = repository;
            _executionContext = executionContext;
            _cacheService = cacheService;
        }

        public async Task Handle(CreateCategoryCommand command, CancellationToken ct)
        {
            Category category = new Category
            {
                Id = 0,
                Name = command.name,
                Description = command.description,
                UserId = _executionContext.UserId,
                TenantId = _executionContext.TenantId,
                CreatedAt = command.createdAt
            };

            await _repository.AddAsync(category);
            await _cacheService.Invalidate();
        }
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
