using Expenses.Application.repositories;
using Expenses.Application.contexts;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.categories.createCategory
{
    public class CreateCategoryHandler: IRequestHandler<CreateCategoryCommand>
    {
        private readonly IRepository<Category, long> _repository;
        private readonly IExecutionContext _executionContext;

        public CreateCategoryHandler(IRepository<Category, long> repository, IExecutionContext executionContext)
        {
            _repository = repository;
            _executionContext = executionContext;
        }

        public async Task Handle(CreateCategoryCommand command, CancellationToken ct)
        {
            Category category = new Category
            {
                Id = 0,
                Name = command.name,
                Description = command.description,
                UserId = _executionContext.UserId,
                TenantId = _executionContext.TenantId,
                CreatedAt = command.createdAt
            };

            await _repository.AddAsync(category);
        } 
    }
}
