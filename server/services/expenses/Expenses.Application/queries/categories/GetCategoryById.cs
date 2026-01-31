using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Expenses.Application.queries.categories
{
    public sealed record GetCategoryByIdQuery(long CategoryId) : IRequest<Category?>;

    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Category?>
    {
        private readonly IRepository<Category, long> _repository;

        public GetCategoryByIdHandler(IRepository<Category, long> repository)
        {
            _repository = repository;
        }

        public async Task<Category?> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(query.CategoryId);
        }
    }

    public class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
        }
    }
}
