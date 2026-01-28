using Expenses.Application.commands.transactions.createTransaction;
using Expenses.Application.commands.transactions.updateTransaction;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Expenses.Application.Tests.commands.transactions
{
    public class UpdateTransactionCommandTest
    {
        [Fact]
        public async Task Handle_ShouldUpdateATransactionCorrectly()
        {
            var repositoryMock = new Mock<IRepository<Transaction, long>>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Transaction { Id = 1, TenantId = 1, UserId = 1 });

            repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateTransactionHandler(repositoryMock.Object);

            var command = new UpdateTransactionCommand(
                1,
                "transaction",
                10,
                TransactionType.Expense,
                Currency.EUR,
                1,
                DateTime.Now
            );

            await handler.Handle(command, CancellationToken.None);
            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotUpdateIfIdIsInvalid()
        {
            var validator = new UpdateTransactionCommandValidator();
            UpdateTransactionCommand command = new UpdateTransactionCommand(
                0,
                "transaction",
                10,
                TransactionType.Expense,
                Currency.EUR,
                1,
                DateTime.Now
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Handle_ShouldNotUpdateIfCategoryIdIsInvalid()
        {
            var validator = new UpdateTransactionCommandValidator();
            UpdateTransactionCommand command = new UpdateTransactionCommand(
                1,
                "transaction",
                10,
                TransactionType.Expense,
                Currency.EUR,
                -1,
                DateTime.Now
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Handle_ShouldNotUpdateIfAmountIsInvalid()
        {
            var validator = new UpdateTransactionCommandValidator();
            UpdateTransactionCommand command = new UpdateTransactionCommand(
                1,
                "transaction",
                0,
                TransactionType.Expense,
                Currency.EUR,
                1,
                DateTime.Now
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }
    }
}
