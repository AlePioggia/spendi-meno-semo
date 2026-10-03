using ClosedXML.Excel;
using Expenses.Application.queries.transactions;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Expenses.Domain.ValueObjects;
using Expenses.Infrastructure.services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Expenses.Application.Tests.queries.transactions
{
    public class GetTransactionsReportQueryTest
    {
        [Fact]
        public async Task CreateReportAsync_ShouldGenerateWorkbookGroupedByMonthWithSummaryChartsData()
        {
            var transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = 1,
                    Description = "Supermarket",
                    Amount = new Money(65.50m, Currency.EUR),
                    ExpenseType = TransactionType.Expense,
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Food" },
                    Date = new DateTime(2025, 1, 10),
                    IsProxyTransaction = false
                },
                new Transaction
                {
                    Id = 2,
                    Description = "Salary",
                    Amount = new Money(2200m, Currency.EUR),
                    ExpenseType = TransactionType.Income,
                    CategoryId = 2,
                    Category = new Category { Id = 2, Name = "Salary" },
                    Date = new DateTime(2025, 1, 25),
                    IsProxyTransaction = false
                },
                new Transaction
                {
                    Id = 3,
                    Description = "Rent",
                    Amount = new Money(900m, Currency.EUR),
                    ExpenseType = TransactionType.Expense,
                    CategoryId = 3,
                    Category = new Category { Id = 3, Name = "Housing" },
                    Date = new DateTime(2025, 2, 12),
                    IsProxyTransaction = false
                },
                new Transaction
                {
                    Id = 4,
                    Description = "Cinema",
                    Amount = new Money(18m, Currency.EUR),
                    ExpenseType = TransactionType.Expense,
                    CategoryId = 1,
                    Category = new Category { Id = 1, Name = "Food" },
                    Date = new DateTime(2025, 2, 17),
                    IsProxyTransaction = false
                }
            };

            var service = new TransactionReportService(new NullLogger<TransactionReportService>());

            var report = await service.CreateReportAsync(transactions, transactions.Select(t => t.Category).DistinctBy(c => c.Id).ToList());

            using var stream = new MemoryStream(report);
            using var workbook = new XLWorkbook(stream);

            Assert.Contains(workbook.Worksheets, sheet => sheet.Name == "Summary");
            Assert.Contains(workbook.Worksheets, sheet => sheet.Name == "2025-01");
            Assert.Contains(workbook.Worksheets, sheet => sheet.Name == "2025-02");

            var summarySheet = workbook.Worksheet("Summary");
            Assert.Contains("Overview", summarySheet.Cell("A4").GetString());
            Assert.Contains("Monthly Totals", summarySheet.Cell("A6").GetString());
            Assert.Contains("Category Totals", summarySheet.Cell("E6").GetString());
            Assert.DoesNotContain(workbook.Worksheet("Transactions").Row(1).CellsUsed(), cell => cell.GetString() == "Is Proxy");
            Assert.NotEmpty(summarySheet.Pictures);
        }
    }
}
