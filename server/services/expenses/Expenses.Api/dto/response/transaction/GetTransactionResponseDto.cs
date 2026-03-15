using Expenses.Domain.Entities;

namespace Expenses.Api.dto.response.transaction
{
    public record GetTransactionResponseDto(
        long Id,
        string Description,
        decimal Amount,
        string Currency,
        string ExpenseType,
        long CategoryId,
        DateTime Date,
        DateTime CreatedAt,
        bool IsProxyTransaction
    );
}