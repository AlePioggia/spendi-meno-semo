using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.categories.deleteCategory
{
    public class DeleteCategoryCommandValidator: AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0)
                .WithMessage("CategoryId is required.");
            RuleFor(x => x.tenantId)
                .GreaterThan(0)
                .WithMessage("TenantId is required.");
            RuleFor(x => x.userId)
                .GreaterThan(0)
                .WithMessage("UserId is required.");
        }
    }
}
