using Expenses.Domain.Entities;
using Expenses.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Expenses.Domain
{
    public class ValueObjectsTests
    {
        [Fact]
        public void MoneyObject_Should_Be_Created_Correctly()
        {
            var MoneyObject = new Money(100, Currency.EUR);
            Assert.Equal(100, MoneyObject.Amount);
            Assert.Equal(Currency.EUR, MoneyObject.Currency);
        }
    }
}
