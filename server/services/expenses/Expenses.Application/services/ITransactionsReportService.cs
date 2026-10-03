using Expenses.Domain.Entities;

namespace Expenses.Application.services
{
    public interface ITransactionsReportService
    {
        Task<byte[]> CreateReportAsync(
            IEnumerable<Transaction> transactions,
            IEnumerable<Category>? categories = null,
            CancellationToken cancellationToken = default);
    }
}
