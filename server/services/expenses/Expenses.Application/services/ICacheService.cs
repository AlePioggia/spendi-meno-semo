using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.services
{
    public interface ICacheService<T>
    {
        Task<T> GetOrCreate(Func<CancellationToken, Task<T>> cacheDelegate);
        Task<T> Invalidate();
    }
}
