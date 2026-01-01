using Expenses.Domain.Entities;
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
                Description = "Food" 
            };
            Assert.Equal(1, category.Id);
            Assert.Equal("Food", category.Description);
        }
    }
}
