using Expenses.Application.commands.recurringTransactions;
using Expenses.Application.contexts;
using Expenses.Application.repositories;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Expenses.Application.Tests.commands.recurringTransactions
{
    public class CreateRecurringTransactionCommandTest
    {
        [Fact]
        public async Task Handle_ShouldCreateARecurringTransactionCorrectly()
        {
            var repositoryMock = new Mock<IRepository<RecurringOperation, long>>();
            var executionContextMock = new Mock<IExecutionContext>();
            var cacheServiceMock = new Mock<ICacheService<RecurringOperation>>();
            executionContextMock.SetupGet(x => x.UserId).Returns("");
            executionContextMock.SetupGet(x => x.TenantId).Returns(1);

            repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<RecurringOperation>()))
                .Callback<RecurringOperation>(t => t.Id = 1)
                .Returns(Task.CompletedTask);

            var command = new CreateRecurringTransactionCommand(
                RecurringDescription: "recurring name",
                TransactionDescription: "transaction description",

                Currency: Currency.EUR,
                Amount: 100m,
                TransactionType: TransactionType.Expense,
                CategoryId: 1,

                RecurringOperationFrequency: RecurringOperationFrequency.Monthly,
                StartDate: new DateTime(2024, 1, 1),
                EndDate: new DateTime(2024, 12, 31),
                TransactionDate: DateTime.Now
            );


            var handler = new CreateRecurringTransactionCommandHandler(
                repositoryMock.Object,
                executionContextMock.Object,
                cacheServiceMock.Object
            );

            await handler.Handle(command, CancellationToken.None);
            repositoryMock.Verify(r => r.AddAsync(It.IsAny<RecurringOperation>()), Times.Once);
        }
    }
}
