using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using MediatR;
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
        long CategoryId,
        DateTime Date            
    ): IRequest;
}