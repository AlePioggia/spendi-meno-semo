namespace Expenses.Domain.Tests;

public class UnitTest1
{
    [Fact]
    public void MoneyObject_Should_Be_Created_Correctly()
    {
        var MoneyObject = new Money() { 100, Currency.EUR };
        Assert.Equal(100, MoneyObject.Amount);
        Assert.Equal(Currency.EUR, MoneyObject.Currency);
    }
}
