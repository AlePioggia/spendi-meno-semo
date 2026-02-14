namespace Expenses.Application.contexts
{
    public interface IExecutionContext
    {
        long TenantId { get; }
        string UserId { get; }
    }

    public sealed class ExecutionContext : IExecutionContext
    {
        public long TenantId { get; set; } = 1;
        public string UserId { get; set; } = "";
    }
}
