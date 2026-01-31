namespace Expenses.Api.dto.response.recurringTransaction
{
    public sealed record GetRecurringTransactionResponseDto(
        long Id,
        string RecurringDescription,
        string Frequency,
        DateTime StartDate,
        DateTime EndDate,
        long CategoryId,
        string TransactionDescription,
        decimal Amount,
        string Currency,
        string TransactionType,
        DateTime TransactionDate,
        DateTime CreatedAt
    );
}
