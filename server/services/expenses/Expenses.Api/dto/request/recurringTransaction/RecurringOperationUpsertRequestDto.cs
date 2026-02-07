namespace Expenses.Api.dto.request.recurringTransaction
{
    public sealed record TransactionTemplateUpsertRequestDto(
        string Description,
        decimal Amount,
        string Currency,
        string TransactionType,
        DateTime Date
    );

    public sealed record RecurringOperationUpsertRequestDto(
        long? Id,
        string Description,
        string Frequency,
        DateTime StartDate,
        DateTime EndDate,
        long CategoryId,
        TransactionTemplateUpsertRequestDto Template
    );
}
