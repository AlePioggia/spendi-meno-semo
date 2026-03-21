using EventProcessor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventProcessor.Application.Repositories
{
    public interface IRepository
    {
        Task AddAsync(StoredEvent storedEvent);
    }
}
