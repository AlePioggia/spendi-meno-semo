namespace Expenses.Application.contexts
{
    public interface IExecutionContext
    {
        long TenantId { get; }
        long UserId { get; }
    }

    public sealed class ExecutionContext : IExecutionContext
    {
        public long TenantId { get; set; } = 1;
        public long UserId { get; set; } = 1;
    }
}
