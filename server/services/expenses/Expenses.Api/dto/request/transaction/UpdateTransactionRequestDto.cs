namespace Expenses.Api.dto.request.transaction
{
    public sealed record UpdateTransactionRequestDto(
        long Id,
        string Description,
        decimal Amount,
        string TransactionType,
        string Currency,
        DateTime Date,
        long CategoryId,
        bool IsProxyTransaction = false
    );
}