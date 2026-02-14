namespace Expenses.Api.dto.response.transaction
{
    public record GetCategoryResponseDto(
        long Id,
        string Name,
        string Description,
        string UserId,
        long TenantId,
        DateTime CreatedAt
    );
}
