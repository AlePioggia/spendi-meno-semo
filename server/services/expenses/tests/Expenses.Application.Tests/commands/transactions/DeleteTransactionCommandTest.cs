using Expenses.Application.commands.transactions;
using Expenses.Application.repositories;
using Expenses.Application.services;
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
            var transaction = new Transaction { Id = 1};
            var repositoryMock = new Mock<IRepository<Transaction, long>>();
            var cacheServiceMock = new Mock<ICacheService<Transaction>>();
            repositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(transaction);

            repositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .Callback<Transaction>(t => t.Status = 1) 
                .Returns(Task.CompletedTask);

            var handler = new DeleteTransactionHandler(repositoryMock.Object, cacheServiceMock.Object);
            var command = new DeleteTransactionCommand(
                1
            );
            await handler.Handle(command, CancellationToken.None);

            repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Transaction>()), Times.Once);
            Assert.Equal(1, transaction.Status);
        }

        [Fact]
        public async Task Handle_CommandShouldFailWhenIdIsLessThanZero()
        {
            var validator = new DeleteTransactionCommandValidator();
            DeleteTransactionCommand command = new DeleteTransactionCommand(
                -1
            );
            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }
    }
}
