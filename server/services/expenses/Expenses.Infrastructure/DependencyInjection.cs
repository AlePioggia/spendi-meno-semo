using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using Expenses.Infrastructure.persistence;
using Expenses.Infrastructure.repositories;
using Expenses.Infrastructure.scheduled;
using Expenses.Infrastructure.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<TransactionsDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
            services.AddScoped<RecurringOperationRepository>();
            services.AddScoped<IRepository<RecurringOperation, long>, RecurringOperationRepository>();
            services.AddScoped<RecurringTransactionsJobRunner>();
            services.AddScoped(typeof(ICacheService<>), typeof(CacheService<>));

            return services;
        }

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<TransactionsDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
            services.AddScoped<RecurringOperationRepository>();
            services.AddScoped<IRepository<RecurringOperation, long>, RecurringOperationRepository>();
            services.AddScoped<RecurringTransactionsJobRunner>();
            services.AddScoped(typeof(ICacheService<>), typeof(CacheService<>));

            return services;
        }
    }
}
