using EventProcessor.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventProcessor.Domain.Entities
{
    public class StoredEvent
    {
        public long Id { get; set; }
        /// <summary>
        /// identifies the specific request that can be used to correlate multiple events
        /// that belong to the same request instance, and can have different processing states
        /// </summary>
        public required string RequestId { get; set; }
        /// <summary>
        /// Identifies the command type, it specifies the operation that needs to be completed
        /// </summary>
        public long OperationTypeId { get; set; }
        /// <summary>
        /// It defines in which elaboration state the event is,
        /// for example: unprocessed, processed, completed, error, etc.
        /// </summary>
        public required long ProcessingStateId { get; set; }
        /// <summary>
        /// Event sourcing tracker, that allows to track the number of times an event has been processed,
        /// and makes you able to retrieve the last version
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// String that contains metadata reguarding the operation requested, in order to understand better
        /// how to handle it
        /// </summary>
        public string? Payload { get; set; }
        public DateTime CreatedAt { get; set; }
        public required OperationType CommandType { get; set; }
        public required ProcessingState ProcessingState { get; set; }
    }
}