using Expenses.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Expenses.Application.commands.transactions.createTransaction
{
    public class CreateTransactionCommandValidator: AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator() 
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.CategoryId)
                .LessThan(0)
                .WithMessage("CategoryId is required.");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required.");

            RuleFor(x => x.UserId)
                .LessThan(0)
                .WithMessage("UserId is required.");
            
            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot exceed 500 characters.");
            
            RuleFor(x => x.TenantId)
                .GreaterThan(0)
                .WithMessage("TenantId is required.");
        }

    }
}
