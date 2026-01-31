using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.recurringTransactions.createRecurringTransaction
{
    public sealed record CreateRecurringTransactionCommand(
        string RecurringDescription,
        string TransactionDescription,
        Currency Currency,
        decimal Amount,
        TransactionType TransactionType,
        long CategoryId,
        RecurringOperationFrequency RecurringOperationFrequency,
        DateTime StartDate,
        DateTime EndDate,
        DateTime TransactionDate
    ) : IRequest;
}
