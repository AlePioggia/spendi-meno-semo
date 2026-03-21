using EventProcessor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventProcessor.Domain.Entities
{
    public class ProcessingState
    {
        public long Id { get; set; }
        public required string State { get; set; }
    }
}
