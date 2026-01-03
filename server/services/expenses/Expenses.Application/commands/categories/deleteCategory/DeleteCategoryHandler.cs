using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.categories.deleteCategory
{
    public class DeleteCategoryHandler
    {
        private readonly IRepository<Category, long> _repository;

        public DeleteCategoryHandler(IRepository<Category, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteCategoryCommand command)
        {
            Category category = new Category
            {
                Id = command.id,
                UserId = command.userId,
                TenantId = command.tenantId
            };

            await _repository.DeleteAsync(
                category
            );
        }
    }
}
