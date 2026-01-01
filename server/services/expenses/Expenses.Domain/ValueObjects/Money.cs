using Expenses.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Domain.Entities
{
    public class Money
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public Money(decimal amount = 0, Currency currency = Currency.EUR)
        {
            if (!IsPositiveAmount(amount))
            {
                throw new ArgumentException("Amount must be a positive value.");
            } 
            Amount = amount;
            Currency = currency;
        }

        private bool IsPositiveAmount(decimal amount)
        {
            return amount >= 0;
        }
    }
}
