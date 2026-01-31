using Expenses.Domain.Entities.enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Domain.Entities
{
    public class RecurringOperation: BaseEntity<long>
    {
        public string? Description { get; set; }
        public RecurringOperationFrequency Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public long TemplateId { get; set; }
        public TransactionTemplate Template { get; set; } = default!;

        public long CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
