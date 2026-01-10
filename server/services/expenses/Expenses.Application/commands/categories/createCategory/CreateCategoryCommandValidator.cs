using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.categories.createCategory
{
    public class CreateCategoryCommandValidator: AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.tenantId)
                .GreaterThan(-1)
                .WithMessage("TenantId is required.");

            RuleFor(x => x.userId)
                .GreaterThan(-1)
                .WithMessage("UserId is required.");
        }
    }
}
