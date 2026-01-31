using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;

namespace Expenses.Domain
{
    public class DomainTests
    {
        [Fact]
        public void TransactionObject_Should_Be_Created_Correctly()
        {
            Transaction expense = new Transaction
            {
                Id = 1,
                Description = "Description",
                Amount = new Money(100, Currency.USD),
                ExpenseType = TransactionType.Expense,
                UserId = 0,
                TenantId = 0,
                CategoryId = 0,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
            };

            Assert.Equal(1, expense.Id);
            Assert.Equal("Description", expense.Description);
            Assert.Equal(100, expense.Amount?.Amount);
            Assert.Equal(Currency.USD, expense.Amount?.Currency);
            Assert.Equal(TransactionType.Expense, expense.ExpenseType);
            Assert.Equal(0, expense.UserId);
            Assert.Equal(0, expense.TenantId);
            Assert.Equal(0, expense.CategoryId);
            Assert.True((DateTime.UtcNow - expense.Date).TotalSeconds < 5);
            Assert.True((DateTime.UtcNow - expense.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void TransactionObject_Should_Throw_Exception_For_Negative_Amount()
        {
            Assert.Throws<ArgumentException>(() => new Money(-50, Currency.USD));
        }

        [Fact]
        public void CategoryObject_Should_Be_Created_Correctly()
        {
            Category category = new Category
            {
                Id = 1,
                Name = "Food",
                Description = "Indicates money spent on food!",
                CreatedAt = DateTime.UtcNow
            };
            Assert.Equal(1, category.Id);
            Assert.Equal("Indicates money spent on food!", category.Description);
        }

        [Fact]
        public void RecurringOperationFrequency_Enum_Should_Have_Correct_Values()
        {
            Assert.Equal(0, (int)RecurringOperationFrequency.Daily);
            Assert.Equal(1, (int)RecurringOperationFrequency.Weekly);
            Assert.Equal(2, (int)RecurringOperationFrequency.Monthly);
            Assert.Equal(3, (int)RecurringOperationFrequency.Yearly);
        }

        [Fact]
        public void TransactionType_Enum_Should_Have_Correct_Values()
        {
            Assert.Equal(0, (int)TransactionType.Expense);
            Assert.Equal(1, (int)TransactionType.Income);
        }

        [Fact]
        public void RecurringOperation_Entity_Should_Have_Correct_Properties()
        {
            RecurringOperation recurringOperation = new RecurringOperation
            {
                Id = 1,
                Description = "Monthly Subscription",
                Frequency = RecurringOperationFrequency.Monthly,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2023, 12, 31),
                TemplateId = 0,
                CreatedAt = DateTime.UtcNow
            };
            Assert.Equal(1, recurringOperation.Id);
            Assert.Equal("Monthly Subscription", recurringOperation.Description);
            Assert.Equal(RecurringOperationFrequency.Monthly, recurringOperation.Frequency);
            Assert.Equal(new DateTime(2023, 1, 1), recurringOperation.StartDate);
            Assert.Equal(new DateTime(2023, 12, 31), recurringOperation.EndDate);
        }
    }
}
