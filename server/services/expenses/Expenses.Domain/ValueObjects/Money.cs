using Expenses.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Domain.Entities
{
    public class Money
    {
        public long Amount { get; set; }
        public Currency Currency { get; set; }

        public Money(long amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
}
