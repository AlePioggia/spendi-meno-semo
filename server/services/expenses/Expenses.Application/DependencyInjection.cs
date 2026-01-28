using FluentValidation;
using Expenses.Application.contexts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Expenses.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddScoped<Expenses.Application.contexts.ExecutionContext>();
            services.AddScoped<IExecutionContext>(sp => sp.GetRequiredService<Expenses.Application.contexts.ExecutionContext>());

            return services;
        }
    }
}
