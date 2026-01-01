using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Domain.Entities
{
    public class Category
    {
        public long Id { get; set; }
        public string? Description { get; set; }
    }
}
