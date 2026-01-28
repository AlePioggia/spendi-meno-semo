using Expenses.Application.contexts;

namespace Expenses.Api.middleware
{
    public sealed class ExecutionContextMiddleware
    {
        private readonly RequestDelegate _next;

        public ExecutionContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, Expenses.Application.contexts.ExecutionContext executionContext)
        {
            executionContext.UserId = 1;
            executionContext.TenantId = 1;

            await _next(httpContext);
        }
    }
}
