using Expenses.Domain.Entities;

namespace Expenses.Api.dto.request.transaction
{
    public sealed record CreateTransactionRequestDto(
        string? Description,
        decimal Amount,
        string? Currency,
        string? TransactionType,
        long CategoryId,
        DateTime Date,
        bool IsProxyTransaction = false
    );
}