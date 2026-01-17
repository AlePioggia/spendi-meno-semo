namespace Expenses.Api.dto.response.transaction
{
    public record GetCategoryResponseDto(
        long Id,
        string Name,
        string Description,
        long UserId,
        long TenantId,
        DateTime CreatedAt
    );
}
