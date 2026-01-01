using Expenses.Domain.Entities;
using Expenses.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.createTransaction
{
    public sealed record CreateTransactionCommand(
        string? Description,
        decimal Amount,
        Currency Currency,
        TransactionType ExpenseType,
        long UserId,
        long TenantId,
        long CategoryId,
        DateTime Date            
    );
}