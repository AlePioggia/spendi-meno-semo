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
                .Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateTransactionHandler(repositoryMock.Object);

            var command = new UpdateTransactionCommand(
                1,
                "transaction",
                10,
                TransactionType.Expense,
                1,
                DateTime.Now,
                1,
                1
            );

            await handler.Handle(command);
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
                1,
                DateTime.Now,
                1,
                1
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Handle_ShouldNotUpdateIfTenantIdIsInvalid()
        {
            var validator = new UpdateTransactionCommandValidator();
            UpdateTransactionCommand command = new UpdateTransactionCommand(
                1,
                "transaction",
                10,
                TransactionType.Expense,
                -1,
                DateTime.Now,
                1,
                1
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Handle_ShouldNotUpdateIfUserIdIsInvalid()
        {
            var validator = new UpdateTransactionCommandValidator();
            UpdateTransactionCommand command = new UpdateTransactionCommand(
                1,
                "transaction",
                10,
                TransactionType.Expense,
                1,
                DateTime.Now,
                -1,
                1
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
                1,
                DateTime.Now,
                1,
                1
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }
    }
}
