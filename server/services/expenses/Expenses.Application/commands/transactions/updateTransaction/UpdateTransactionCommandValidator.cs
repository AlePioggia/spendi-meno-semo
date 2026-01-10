using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.updateTransaction
{
    public class UpdateTransactionCommandValidator: AbstractValidator<UpdateTransactionCommand>
    {
        public UpdateTransactionCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0).WithMessage("Transaction Id must be greater than zero.");
            RuleFor(x => x.tenantId)
                .GreaterThan(0).WithMessage("Tenant Id must be greater than zero.");
            RuleFor(x => x.userId)
                .GreaterThan(0).WithMessage("User Id must be greater than zero.");
            RuleFor(x => x.date)
                .NotNull().WithMessage("Date must be provided.");
            RuleFor(x => x.categoryId)
                .GreaterThan(0).WithMessage("Category Id must be greater than zero.");
            RuleFor(x => x.amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
