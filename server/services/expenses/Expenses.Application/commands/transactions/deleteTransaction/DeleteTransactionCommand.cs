using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.transactions.deleteTransaction
{
    public record DeleteTransactionCommand(long id) : IRequest;
}
