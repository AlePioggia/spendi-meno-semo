using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.categories.getCategoryById
{
    public class GetCategoryByIdQueryValidator: AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdQueryValidator()
        {
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("TransactionId must be greater than 0.");
        }
    }
}
