namespace Expenses.Api.dto.response.recurringTransaction
{
    public sealed record TransactionTemplateResponseDto(
        string Description,
        decimal Amount,
        string Currency,
        string TransactionType,
        DateTime Date
    );

    public sealed record GetRecurringTransactionResponseDto(
        long Id,
        string Description,
        string Frequency,
        DateTime StartDate,
        DateTime EndDate,
        long CategoryId,
        DateTime CreatedAt,
        TransactionTemplateResponseDto Template
    );
}
