using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Domain.Entities
{
    public class Category
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long UserId { get; set; }
        public long TenantId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
            = new List<Transaction>();
        public int Status { get; set; }

        public void Delete()
        {
            Status = 1;
        }
    }
}
