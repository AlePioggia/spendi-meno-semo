using Expenses.Application.queries.recurringTransactions;
using Expenses.Application.repositories;
using Expenses.Domain.Entities;
using Moq;

namespace Expenses.Application.Tests.queries.recurringTransactions
{
    public class GetRecurringTransactionsQueryTest
    {
        [Fact]
        public async Task Handle_ShouldReturnAllRecurringTransactions()
        {
            var repositoryMock = new Mock<IRepository<RecurringOperation, long>>();

            repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<RecurringOperation> { new RecurringOperation { Id = 1 } });

            var handler = new GetRecurringTransactionsHandler(repositoryMock.Object);

            var result = await handler.Handle(new GetRecurringTransactionsQuery(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Single(result!);
        }

        [Fact]
        public async Task Handle_ShouldReturnRecurringTransactionById()
        {
            var repositoryMock = new Mock<IRepository<RecurringOperation, long>>();

            repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new RecurringOperation { Id = 1 });

            var handler = new GetRecurringTransactionByIdHandler(repositoryMock.Object);

            var result = await handler.Handle(new GetRecurringTransactionByIdQuery(1), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public void GetByIdValidator_ShouldFailWhenIdIsInvalid()
        {
            var validator = new GetRecurringTransactionByIdQueryValidator();
            var result = validator.Validate(new GetRecurringTransactionByIdQuery(0));
            Assert.False(result.IsValid);
        }
    }
}
