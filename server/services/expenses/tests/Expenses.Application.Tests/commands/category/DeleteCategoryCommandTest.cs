using Expenses.Application.commands.categories;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.Tests.commands.category
{
    public class DeleteCategoryCommandTest
    {
        [Fact]
        public async Task Handle_ShouldDeleteAtransactionCorrectly()
        {
            var category = new Category { Id = 1 };
            var repositoryMock = new Mock<IRepository<Category, long>>();
            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(category);

            repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .Callback<Category>(c => c.Status = 1)
                .Returns(Task.CompletedTask);

            var handler = new DeleteCategoryHandler(repositoryMock.Object);
            var command = new DeleteCategoryCommand(
                1
            );
            
            await handler.Handle(command, CancellationToken.None);

            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
            Assert.Equal(1, category.Status);
        }

        [Fact]
        public async Task Handle_CommandShouldFailWhenIdIsLessThanZero()
        {
            var validator = new DeleteCategoryCommandValidator();
            DeleteCategoryCommand command = new DeleteCategoryCommand(
                -1
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }
    }
}
