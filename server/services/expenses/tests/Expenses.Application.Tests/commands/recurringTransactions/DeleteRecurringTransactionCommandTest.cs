using Expenses.Application.commands.recurringTransactions;
using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using Moq;

namespace Expenses.Application.Tests.commands.recurringTransactions
{
    public class DeleteRecurringTransactionCommandTest
    {
        [Fact]
        public async Task Handle_ShouldDeleteARecurringTransactionCorrectly()
        {
            var recurring = new RecurringOperation { Id = 1 };
            var repositoryMock = new Mock<IRepository<RecurringOperation, long>>();
            var cacheServiceMock = new Mock<ICacheService<RecurringOperation>>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(recurring);

            repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<RecurringOperation>()))
                .Callback<RecurringOperation>(ro => ro.Status = 1)
                .Returns(Task.CompletedTask);

            var handler = new DeleteRecurringTransactionHandler(repositoryMock.Object, cacheServiceMock.Object);

            await handler.Handle(new DeleteRecurringTransactionCommand(1), CancellationToken.None);

            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RecurringOperation>()), Times.Once);
            Assert.Equal(1, recurring.Status);
        }

        [Fact]
        public void Validator_ShouldFailWhenIdIsInvalid()
        {
            var validator = new DeleteRecurringTransactionCommandValidator();
            var result = validator.Validate(new DeleteRecurringTransactionCommand(0));
            Assert.False(result.IsValid);
        }
    }
}
