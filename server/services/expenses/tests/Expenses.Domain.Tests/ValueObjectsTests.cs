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

        [Fact]
        public void MoneyObject_Should_ThrowException_For_Negative_Amount()
        {
            Assert.Throws<ArgumentException>(() => new Money(-50, Currency.EUR));
        }

        [Fact]
        public void MoneyObject_Should_Default_To_EUR_If_No_Currency_Provided()
        {
            var MoneyObject = new Money(200);
            Assert.Equal(200, MoneyObject.Amount);
            Assert.Equal(Currency.EUR, MoneyObject.Currency);
        }

        [Fact]
        public void MoneyObject_Should_Default_To_Zero_If_No_Amount_Provided()
        {
            var MoneyObject = new Money();
            Assert.Equal(0, MoneyObject.Amount);
            Assert.Equal(Currency.EUR, MoneyObject.Currency);
        }
    }
}
