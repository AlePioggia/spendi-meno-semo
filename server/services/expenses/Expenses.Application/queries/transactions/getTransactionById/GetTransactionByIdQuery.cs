using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.transactions.getTransactionById
{
    public record GetTransactionByIdQuery(
        long TransactionId
    ): IRequest<Transaction?>;
}
