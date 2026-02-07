using Expenses.Application.queries.transactions;
using Expenses.Application.repositories;
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
        public async Task Handle_ShouldCorrectlyReturnTransactionsWgenExist()
        {
            var repositoryMock = new Mock<IRepository<Transaction, long>>();
            long firstId = 1;
            long secondId = 2;

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(new Transaction
            {
                Id = firstId,
                Description = "transazione",
                Amount = new Money(),
                ExpenseType = TransactionType.Expense,
                UserId = firstId,
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
                UserId = secondId,
                TenantId = secondId,
                CategoryId = secondId,
                Date = DateTime.Now,
                CreatedAt = DateTime.Now,
            });

            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(transactions);

            var handler = new GetTransactionsHandler(repositoryMock.Object);

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

            List<Transaction> transactions = new List<Transaction>();

            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(transactions);

            var handler = new GetTransactionsHandler(repositoryMock.Object);

            var command = new GetTransactionsQuery();

            List<Transaction>? result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(transactions, result);
            Assert.True(transactions == result);
        }
    }
}
