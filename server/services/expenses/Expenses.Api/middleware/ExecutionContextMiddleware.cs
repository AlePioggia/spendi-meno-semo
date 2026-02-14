using Expenses.Application.contexts;
using System.Security.Claims;

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
            var subject = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            executionContext.UserId = subject ?? "";
            executionContext.TenantId = 1;

            await _next(httpContext);
        }
    }
}
