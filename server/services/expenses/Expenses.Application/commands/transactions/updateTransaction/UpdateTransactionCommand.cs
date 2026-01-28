using Expenses.Domain.Entities;
using Expenses.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.updateTransaction
{
    public record UpdateTransactionCommand(
        long id,
        string description,
        decimal amount,
        TransactionType transactionType,
        Currency currency,
        long categoryId,
        DateTime date
    ): IRequest;
}
