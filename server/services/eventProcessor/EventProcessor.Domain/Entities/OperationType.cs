using System;
using System.Collections.Generic;
using System.Text;

namespace EventProcessor.Domain.Entities
{
    public class OperationType
    {
        public long Id { get; set; }
        public required string Type { get; set; }
    }
}
