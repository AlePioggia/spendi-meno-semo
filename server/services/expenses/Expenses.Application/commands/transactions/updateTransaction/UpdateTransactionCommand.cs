using Expenses.Domain.Entities;
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
        long categoryId,
        DateTime date,
        long userId,
        long tenantId
    ): IRequest;
}
