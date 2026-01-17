using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Domain.Entities
{
    public class Transaction
    {
        public long Id { get; set; }
        public string? Description { get; set; }
        public Money? Amount { get; set; }
        public TransactionType ExpenseType { get; set; }
        public long UserId { get; set; }
        public long TenantId { get; set; }
        public long CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
        public void Delete()
        {
            Status = 1;
        }
    }
}
