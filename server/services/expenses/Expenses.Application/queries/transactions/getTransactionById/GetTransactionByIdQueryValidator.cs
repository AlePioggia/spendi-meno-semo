using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.transactions.getTransactionById
{
    public class GetTransactionByIdQueryValidator: AbstractValidator<GetTransactionByIdQuery>
    {
        public GetTransactionByIdQueryValidator()
        {
            RuleFor(x => x.TransactionId)
                .GreaterThan(0).WithMessage("TransactionId must be greater than 0.");
            RuleFor(x => x.TenantId)
                .GreaterThan(0).WithMessage("TenantId must be greater than 0.");
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");
        }
    }
}
