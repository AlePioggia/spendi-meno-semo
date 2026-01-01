using Expenses.Application.queries.transactions.getTransactionById;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Expenses.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.Tests.queries.transactions
{
    public class GetTransactionByIdQueryTest
    {
        [Fact]
        public async Task Handle_ShouldCorrectlyReturnTransactionWhenExists()
        {
            var repositoryMock = new Mock<IRepository<Transaction, long>>();
            long fakeId = 1;

            Transaction transaction = new Transaction()
            {
                Id = fakeId,
                Description = "transazione",
                Amount = new Money(),
                ExpenseType = TransactionType.Expense,
                UserId = fakeId,
                TenantId = fakeId,
                CategoryId = fakeId,
                Date = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            repositoryMock
                .Setup(repository => repository.GetByIdAsync(fakeId).Result)
                .Returns(transaction);

            var handler = new GetTransactionByIdHandler(repositoryMock.Object);

            var command = new GetTransactionByIdQuery(
                fakeId,
                fakeId,
                fakeId
            );

            Transaction result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(transaction, result);
            Assert.True(transaction == result);
        }

        [Fact]
        public async Task Handle_ShouldCorrectlyNotReturnTransactionWhenItDoesntExist()
        {
            var repositoryMock = new Mock<IRepository<Transaction, long>>();
            long fakeId = 1;

            Transaction transaction = new Transaction()
            {
                Id = fakeId,
                Description = "transazione",
                Amount = new Money(),
                ExpenseType = TransactionType.Expense,
                UserId = fakeId,
                TenantId = fakeId,
                CategoryId = fakeId,
                Date = DateTime.Now,
                CreatedAt = DateTime.Now,
            };

            repositoryMock
                .Setup(repository => repository.GetByIdAsync(1000).Result)
                .Returns(transaction);

            var handler = new GetTransactionByIdHandler(repositoryMock.Object);

            var command = new GetTransactionByIdQuery(
                fakeId,
                fakeId,
                fakeId
            );

            Transaction result = await handler.Handle(command, CancellationToken.None);

            Assert.Null(result);
        }
    }
}
