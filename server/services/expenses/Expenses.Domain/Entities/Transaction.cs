using Expenses.Domain.Entities.enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Domain.Entities
{
    public class Transaction : BaseEntity<long>
    {
        public string? Description { get; set; }
        public Money? Amount { get; set; }
        public TransactionType ExpenseType { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public DateTime Date { get; set; }
    }
}
