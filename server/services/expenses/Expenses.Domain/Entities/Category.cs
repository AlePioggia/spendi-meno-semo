using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Domain.Entities
{
    public class Category : BaseEntity<long>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
            = new List<Transaction>();
    }
}
