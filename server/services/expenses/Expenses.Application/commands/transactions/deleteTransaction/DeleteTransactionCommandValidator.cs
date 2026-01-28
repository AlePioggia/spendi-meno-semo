using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.deleteTransaction
{
    public class DeleteTransactionCommandValidator: AbstractValidator<DeleteTransactionCommand>
    {
        public DeleteTransactionCommandValidator()
        {
            RuleFor(x => x.id)
                .GreaterThan(0).WithMessage("Transaction Id must be greater than zero.");
        }
    }
}
