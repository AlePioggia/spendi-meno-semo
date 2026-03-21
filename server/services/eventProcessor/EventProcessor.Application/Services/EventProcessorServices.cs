using EventProcessor.Application.Repositories;
using EventProcessor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventProcessor.Application.Services
{
    public class EventProcessorServices : IEventProcessorServices
    {
        private readonly IRepository _repository;

        public EventProcessorServices(IRepository repository)
        {
            _repository = repository;
        }

        public  async Task CreateSingleStoredEvent(string requestId, int operationId, string? payload, long processingStateId, DateTime receivedAt)
        {
            var storedEvent = new StoredEvent
            {
                RequestId = requestId,
                OperationTypeId = operationId,
                ProcessingStateId = processingStateId,
                Payload = payload,
                CreatedAt = DateTime.UtcNow,
                EventReceivedAt = receivedAt
            };

            await _repository.AddAsync(storedEvent);
        }

        public Task StartElaboration(string requestId, int operationId, string? payload, DateTime receivedAt)
        {
            throw new NotImplementedException();
        }
    }
}
