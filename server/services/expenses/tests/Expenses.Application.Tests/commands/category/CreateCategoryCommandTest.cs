using Expenses.Application.commands.categories;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Expenses.Application.Tests.commands.category
{
    public class CreateCategoryCommandTest
    {
        [Fact]
        public async Task Handle_ShouldCreateACategory()
        {
            var repositoryMock = new Mock<IRepository<Category, long>>();

            repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(t => t.Id = 1)
                .Returns(Task.CompletedTask);

            var command = new CreateCategoryCommand(
                "Food",
                "Indicates money spent on food!",
                1,
                1,
                DateTime.UtcNow
            );

            var handler = new CreateCategoryHandler(repositoryMock.Object);

            await handler.Handle(command, CancellationToken.None);

            repositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);

        } 
    }
}
