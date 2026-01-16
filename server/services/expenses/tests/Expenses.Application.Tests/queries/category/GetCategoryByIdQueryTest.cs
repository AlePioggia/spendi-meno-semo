using Expenses.Application.queries.categories.getCategoryById;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.Tests.queries.category
{
    public class GetCategoryByIdQueryTest
    {
        [Fact]
        public async Task Handle_ShouldCorrectlyReturnCategoryWhenExists()
        {
            var repositoryMock = new Mock<IRepository<Category, long>>();
            long fakeId = 1;
            Category category = new Category()
            {
                Id = fakeId,
                Name = "Food",
                Description = "Indicates money spent on food!",
                UserId = fakeId,
                TenantId = fakeId,
                CreatedAt = DateTime.UtcNow
            };
            repositoryMock
                .Setup(repository => repository.GetByIdAsync(fakeId).Result)
                .Returns(category);
            var handler = new GetCategoryByIdHandler(repositoryMock.Object);
            var command = new GetCategoryByIdQuery(
                fakeId,
                fakeId,
                fakeId
            );
            Category? result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal(category, result);
            Assert.True(category == result);
        }

        [Fact]
        public async Task Handle_ShouldCorrectlyNotReturnCategoryWhenItDoesntExist()
        {
            var repositoryMock = new Mock<IRepository<Category, long>>();
            long fakeId = 1;
            Category category = new Category()
            {
                Id = fakeId,
                Name = "Food",
                Description = "Indicates money spent on food!",
                CreatedAt = DateTime.UtcNow
            };
            repositoryMock
                .Setup(repository => repository.GetByIdAsync(fakeId).Result)
                .Returns((Category?)null);
            var handler = new GetCategoryByIdHandler(repositoryMock.Object);
            var command = new GetCategoryByIdQuery(
                fakeId,
                fakeId,
                fakeId
            );
            Category? result = await handler.Handle(command, CancellationToken.None);
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_ShouldNotValidateIfCategoryIdIsLessThanOne()
        {
            var validator = new GetCategoryByIdQueryValidator();

            var query = new GetCategoryByIdQuery(
                -1, 
                1, 
                1
            );

            var result = await validator.ValidateAsync(query);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetCategoryByIdQuery.CategoryId));
        }

        [Fact]
        public async Task Handle_ShouldNotValidateIfUserIdIsLessThanOne()
        {
            var validator = new GetCategoryByIdQueryValidator();
            var query = new GetCategoryByIdQuery(
                1,
                -1,
                1
            );
            var result = await validator.ValidateAsync(query);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetCategoryByIdQuery.UserId));
        }

        [Fact]
        public async Task Handle_ShouldNotValidateIfTenantIdIsLessThanOne()
        {
            var validator = new GetCategoryByIdQueryValidator();
            var query = new GetCategoryByIdQuery(
                1,
                1,
                -1
            );
            var result = await validator.ValidateAsync(query);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetCategoryByIdQuery.TenantId));
        }
    }
}
