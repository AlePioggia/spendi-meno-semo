using System;
using System.Collections.Generic;
using System.Text;

namespace EventProcessor.Domain.Messages
{
    public class ProcessingCommand
    {
        public required string RequestId { get; set; }
        public int OperationId { get; set; }
        public string? Payload { get; set; }
        public string? CallbackUrl { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
