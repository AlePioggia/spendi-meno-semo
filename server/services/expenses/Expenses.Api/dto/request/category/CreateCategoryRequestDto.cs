namespace Expenses.Api.dto.request.category
{
    public sealed record CreateCategoryRequestDto(
        string Name,
        string Description
    );
}