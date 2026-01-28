using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.commands.categories.deleteCategory
{
    public class DeleteCategoryHandler: IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IRepository<Category, long> _repository;

        public DeleteCategoryHandler(IRepository<Category, long> repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteCategoryCommand command, CancellationToken ct)
        {
            Category? entity = await _repository.GetByIdAsync(command.id);
            if (entity is null)
            {
                return;
            }
            entity.Delete();
            await _repository.UpdateAsync(
                entity
            );
        }
    }
}
