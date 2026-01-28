using Expenses.Application.queries.transactions.getTransactionById;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Expenses.Application.queries.categories.getCategoryById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Category?>
    {
        private readonly IRepository<Category, long> _repository; 

        public GetCategoryByIdHandler(IRepository<Category, long> repository)
        {
            _repository = repository;
        }

        public async Task<Category?> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await _repository.GetByIdAsync(query.CategoryId);
        }
    }
}
