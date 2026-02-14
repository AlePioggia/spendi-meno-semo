using Expenses.Application.commands.recurringTransactions;
using Expenses.Application.contexts;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using Moq;

namespace Expenses.Application.Tests.commands.recurringTransactions
{
    public class UpdateRecurringTransactionCommandTest
    {
        [Fact]
        public async Task Handle_ShouldUpdateARecurringTransactionCorrectly()
        {
            var repositoryMock = new Mock<IRepository<RecurringOperation, long>>();
            var executionContextMock = new Mock<IExecutionContext>();
            executionContextMock.SetupGet(x => x.UserId).Returns("");
            executionContextMock.SetupGet(x => x.TenantId).Returns(1);

            var existing = new RecurringOperation
            {
                Id = 1,
                CategoryId = 1,
                TemplateId = 10,
                Template = new TransactionTemplate { Id = 10, UserId = "", TenantId = 1 },
                UserId = "",
                TenantId = 1
            };

            repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existing);

            repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<RecurringOperation>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateRecurringTransactionHandler(repositoryMock.Object, executionContextMock.Object);

            var command = new UpdateRecurringTransactionCommand(
                1,
                "recurring updated",
                "transaction updated",
                Currency.EUR,
                20m,
                TransactionType.Expense,
                1,
                RecurringOperationFrequency.Monthly,
                new DateTime(2024, 1, 1),
                new DateTime(2024, 12, 31),
                DateTime.UtcNow
            );

            await handler.Handle(command, CancellationToken.None);

            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RecurringOperation>()), Times.Once);
        }

        [Fact]
        public void Validator_ShouldFailWhenIdIsInvalid()
        {
            var validator = new UpdateRecurringTransactionCommandValidator();
            var command = new UpdateRecurringTransactionCommand(
                0,
                "rec",
                "tx",
                Currency.EUR,
                10m,
                TransactionType.Expense,
                1,
                RecurringOperationFrequency.Monthly,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow
            );

            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validator_ShouldFailWhenStartDateIsAfterEndDate()
        {
            var validator = new UpdateRecurringTransactionCommandValidator();
            var command = new UpdateRecurringTransactionCommand(
                1,
                "rec",
                "tx",
                Currency.EUR,
                10m,
                TransactionType.Expense,
                1,
                RecurringOperationFrequency.Monthly,
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow
            );

            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }
    }
}
