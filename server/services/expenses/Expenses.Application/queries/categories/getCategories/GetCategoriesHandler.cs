using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.queries.categories.getCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<Category>?>
    {
        private readonly IRepository<Category, long> _repository;

        public GetCategoriesHandler(IRepository<Category, long> repository)
        {
            _repository = repository;
        }

        public async Task<List<Category>?> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync();
        }
    }
}
