using System;
using System.Collections.Generic;
using System.Text;

namespace EventProcessor.Application.Services
{
    public interface IEventProcessorServices
    {
        Task CreateSingleStoredEvent(
            string requestId,
            int operationId,
            string? payload,
            long processingStateId,
            DateTime receivedAt
        );

        Task StartElaboration(
            string requestId,
            int operationId,
            string? payload,
            DateTime receivedAt
        );
    }
}