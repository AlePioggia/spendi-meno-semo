using Expenses.Application.queries.categories;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.Tests.queries.category
{
    public class GetCategoriesQueryTest
    {
        [Fact]
        public async Task Handle_ShouldReturnCategoriesWhenTheyExist()
        {
            var repositoryMock = new Mock<IRepository<Category, long>>();
            long firstId = 1;
            long secondId = 2;
            List<Category> categories = new List<Category>();
            categories.Add(new Category
            {
                Id = firstId,
                Name = "Category 1",
                Description = "Description 1",
                UserId = firstId,
                TenantId = firstId,
                CreatedAt = DateTime.Now,
            });
            categories.Add(new Category
            {
                Id = secondId,
                Name = "Category 2",
                Description = "Description 2",
                UserId = secondId,
                TenantId = secondId,
                CreatedAt = DateTime.Now,
            });
            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(categories);
            var handler = new GetCategoriesHandler(repositoryMock.Object);
            var command = new GetCategoriesQuery();
            List<Category>? result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(categories, result);
            Assert.True(categories == result);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyListWhenNoCategoriesExist()
        {
            var repositoryMock = new Mock<IRepository<Category, long>>();
            List<Category> categories = new List<Category>();
            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(categories);
            var handler = new GetCategoriesHandler(repositoryMock.Object);
            var command = new GetCategoriesQuery();
            List<Category>? result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
