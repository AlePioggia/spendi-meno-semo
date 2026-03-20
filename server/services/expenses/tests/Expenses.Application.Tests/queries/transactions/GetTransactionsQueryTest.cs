using Expenses.Application.queries.transactions;
using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expenses.Application.Tests.queries.transactions
{
    public class GetTransactionsQueryTest
    {
        [Fact]
        public async Task Handle_ShouldCorrectlyReturnTransactionsWhenExist()
        {
            var repositoryMock = new Mock<IRepository<Transaction, long>>();
            var cacheServiceMock = new Mock<ICacheService<List<Transaction>>>();
            long firstId = 1;
            long secondId = 2;

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction
            {
                Id = firstId,
                Description = "transazione",
                Amount = new Money(),
                ExpenseType = TransactionType.Expense,
                UserId = "",
                TenantId = firstId,
                CategoryId = firstId,
                Date = DateTime.Now,
                CreatedAt = DateTime.Now,
            });

            transactions.Add(new Transaction
            {
                Id = secondId,
                Description = "transazione",
                Amount = new Money(),
                ExpenseType = TransactionType.Expense,
                UserId = "",
                TenantId = secondId,
                CategoryId = secondId,
                Date = DateTime.Now,
                CreatedAt = DateTime.Now,
            });

            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(transactions);

            cacheServiceMock
                .Setup(c => c.GetOrCreate(It.IsAny<Func<CancellationToken, Task<List<Transaction>>>>()));

            var handler = new GetTransactionsHandler(repositoryMock.Object, cacheServiceMock.Object);

            var command = new GetTransactionsQuery();


            List<Transaction>? result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(transactions, result);
            Assert.True(transactions == result);
        }

        [Fact]
        public async Task Handle_ShouldCorrectlyReturnEmptyTransactionListWhenEmpty()
        {
            var repositoryMock = new Mock<IRepository<Transaction, long>>();
            var cacheServiceMock = new Mock<ICacheService<List<Transaction>>>();

            List<Transaction> transactions = new List<Transaction>();

            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(transactions);

            cacheServiceMock
                .Setup(c => c.GetOrCreate(It.IsAny<Func<CancellationToken, Task<List<Transaction>>>>()))
                .ReturnsAsync(transactions);

            var handler = new GetTransactionsHandler(repositoryMock.Object, cacheServiceMock.Object);

            var command = new GetTransactionsQuery();

            List<Transaction>? result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(transactions, result);
            Assert.True(transactions == result);
        }
    }
}
