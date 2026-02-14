using Expenses.Application.commands.transactions;
using Expenses.Application.contexts;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using Moq;


namespace Expenses.Application.Tests.commands.transactions
{
    public class CreateTransactionCommandTest
    {
        [Fact]
        public async Task Handle_ShouldCreateATransactionCorrectly()
        {
            var repositoryMock = new Mock<IRepository<Transaction, long>>();
            var executionContextMock = new Mock<IExecutionContext>();
            executionContextMock.SetupGet(x => x.UserId).Returns("");
            executionContextMock.SetupGet(x => x.TenantId).Returns(1);
            long fakeId = 1;

            repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                .Callback<Transaction>(t => t.Id = fakeId)
                .Returns(Task.CompletedTask);

            var handler = new CreateTransactionHandler(repositoryMock.Object, executionContextMock.Object);

            var command = new CreateTransactionCommand(
                    "fake transaction",
                    231,
                    Currency.EUR,
                    TransactionType.Expense,
                    1,
                    DateTime.Now
            );

            await handler.Handle(command, CancellationToken.None);

            repositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CommandShouldFailWhenCategoryIdIsLessThanZero()
        {
            var validator = new CreateTransactionCommandValidator();
            CreateTransactionCommand command = new CreateTransactionCommand(
                "fake transaction",
                231,
                Currency.EUR,
                TransactionType.Expense,
                -1,
                DateTime.Now
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Handle_CommandShouldFailWhenAmountIsLessThanOrEqualToZero()
        {
            var validator = new CreateTransactionCommandValidator();
            CreateTransactionCommand command = new CreateTransactionCommand(
                "fake transaction",
                0,
                Currency.EUR,
                TransactionType.Expense,
                1,
                DateTime.Now
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }
    } 
}