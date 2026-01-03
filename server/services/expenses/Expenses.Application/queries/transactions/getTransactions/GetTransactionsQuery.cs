using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.transactions.getTransactions
{
    public record GetTransactionsQuery(
        long tenantId,
        long userId
    );
}
