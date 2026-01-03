using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.categories
{
    public class CreateCategoryHandler
    {
        private readonly IRepository<Category, long> _repository;

        public CreateCategoryHandler(IRepository<Category, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(CreateCategoryCommand command, CancellationToken ct)
        {
            Category category = new Category
            {
                Id = 0,
                Name = command.name,
                Description = command.description,
                UserId = command.userId,
                TenantId = command.tenantId,
                CreatedAt = command.createdAt
            };

            await _repository.AddAsync(category);
        } 
    }
}
