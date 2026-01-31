using Expenses.Application.commands.recurringTransactions;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;

namespace Expenses.Application.Tests.commands.recurringTransactions
{
    public class CreateRecurringTransactionCommandValidatorTest
    {
        [Fact]
        public void Validator_ShouldFailWhenAmountIsInvalid()
        {
            var validator = new CreateRecurringTransactionCommandValidator();
            var command = new CreateRecurringTransactionCommand(
                "rec",
                "tx",
                Currency.EUR,
                0,
                TransactionType.Expense,
                1,
                RecurringOperationFrequency.Monthly,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow
            );

            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validator_ShouldFailWhenStartDateIsAfterEndDate()
        {
            var validator = new CreateRecurringTransactionCommandValidator();
            var command = new CreateRecurringTransactionCommand(
                "rec",
                "tx",
                Currency.EUR,
                10,
                TransactionType.Expense,
                1,
                RecurringOperationFrequency.Monthly,
                DateTime.UtcNow.AddDays(2),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow
            );

            var result = validator.Validate(command);
            Assert.False(result.IsValid);
        }
    }
}
