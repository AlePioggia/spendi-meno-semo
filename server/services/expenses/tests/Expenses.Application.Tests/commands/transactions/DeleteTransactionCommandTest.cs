using Expenses.Application.commands.transactions.deleteTransaction;
using Expenses.Application.commands.transactions.updateTransaction;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.Tests.commands.transactions
{
    public class DeleteTransactionCommandTest
    {
        [Fact]
        public async Task Handle_ShouldDeleteAtransactionCorrectly()
        {
            var repositoryMock = new Mock<IRepository<Transaction, long>>();
            repositoryMock
                .Setup(r => r.DeleteAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);

            var handler = new DeleteTransactionHandler(repositoryMock.Object);
            var command = new DeleteTransactionCommand(
                1,
                1,
                1
            );

            await handler.Handle(command);
            repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Transaction>()), Times.Once);
        }
    }
}
