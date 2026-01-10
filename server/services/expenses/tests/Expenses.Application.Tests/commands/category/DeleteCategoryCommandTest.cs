using Expenses.Application.commands.categories.deleteCategory;
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
        public class DeleteTransactionCommandTest
        {
            [Fact]
            public async Task Handle_ShouldDeleteAtransactionCorrectly()
            {
                var repositoryMock = new Mock<IRepository<Category, long>>();
                repositoryMock
                    .Setup(r => r.DeleteAsync(It.IsAny<Category>()))
                    .Returns(Task.CompletedTask);

                var handler = new DeleteCategoryHandler(repositoryMock.Object);
                var command = new DeleteCategoryCommand(
                    1,
                    1,
                    1
                );

                await handler.Handle(command);
                repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Once);
            }

            [Fact]
            public async Task Handle_CommandShouldFailWhenTenantIdIsLessThanZero()
            {
                var validator = new DeleteCategoryCommandValidator();
                DeleteCategoryCommand command = new DeleteCategoryCommand(
                    -1,
                    1,
                    1
                );
                var result = validator.Validate(command);
                Assert.False(result.IsValid);
            }

            [Fact]
            public async Task Handle_CommandShouldFailWhenUserIdIsLessThanZero()
            {
                var validator = new DeleteCategoryCommandValidator();
                DeleteCategoryCommand command = new DeleteCategoryCommand(
                    1,
                    -1,
                    1
                );
                var result = validator.Validate(command);
                Assert.False(result.IsValid);
            }

            [Fact]
            public async Task Handle_CommandShouldFailWhenCategoryIdIsLessThanZero()
            {
                var validator = new DeleteCategoryCommandValidator();
                DeleteCategoryCommand command = new DeleteCategoryCommand(
                    1,
                    1,
                    -1
                );
                var result = validator.Validate(command);
                Assert.False(result.IsValid);
            }
        }
    }
}
