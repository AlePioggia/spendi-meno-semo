using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.transactions.getTransactionById
{
    public record GetTransactionByIdQuery(
        long TransactionId,
        long TenantId,
        long UserId
    );
}
