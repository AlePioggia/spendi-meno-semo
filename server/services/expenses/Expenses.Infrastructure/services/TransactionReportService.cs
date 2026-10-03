using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using Expenses.Application.services;
using Expenses.Domain.Entities;
using Expenses.Domain.Entities.enums;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Expenses.Infrastructure.services
{
    public class TransactionReportService : ITransactionsReportService
    {
        private readonly ILogger<TransactionReportService> _logger;

        public TransactionReportService(ILogger<TransactionReportService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> CreateReportAsync(
            IEnumerable<Transaction> transactions,
            IEnumerable<Category>? categories = null,
            CancellationToken cancellationToken = default)
        {
            var orderedTransactions = (transactions ?? Enumerable.Empty<Transaction>())
                .Where(tx => tx is not null)
                .OrderBy(tx => tx.Date)
                .ToList();

            var allCategories = (categories ?? orderedTransactions
                    .Where(tx => tx.Category is not null)
                    .Select(tx => tx.Category)
                    .DistinctBy(category => category.Id))
                .ToList();

            using var workbook = new XLWorkbook();

            AddTransactionsSheet(workbook, orderedTransactions);
            AddMonthlySheets(workbook, orderedTransactions);
            AddSummarySheet(workbook, orderedTransactions, allCategories);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            _logger.LogInformation("Generated Excel report for {TransactionCount} transactions and {CategoryCount} categories.", orderedTransactions.Count, allCategories.Count);

            await Task.CompletedTask;
            return stream.ToArray();
        }

        private static void AddTransactionsSheet(XLWorkbook workbook, IReadOnlyList<Transaction> transactions)
        {
            var sheet = workbook.Worksheets.Add("Transactions");
            sheet.Cell("A1").Value = "Transactions report";
            sheet.Range("A1:F1").Merge();
            sheet.Cell("A1").Style.Font.SetBold();
            sheet.Cell("A1").Style.Font.FontSize = 14;
            sheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78");
            sheet.Cell("A1").Style.Font.FontColor = XLColor.White;
            sheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            sheet.Cell("A3").Value = "Date";
            sheet.Cell("B3").Value = "Description";
            sheet.Cell("C3").Value = "Category";
            sheet.Cell("D3").Value = "Type";
            sheet.Cell("E3").Value = "Amount";
            sheet.Cell("F3").Value = "Currency";

            var row = 4;
            foreach (var transaction in transactions)
            {
                sheet.Cell(row, 1).Value = transaction.Date.Date;
                sheet.Cell(row, 2).Value = transaction.Description ?? "";
                sheet.Cell(row, 3).Value = transaction.Category?.Name ?? GetCategoryName(transaction.CategoryId);
                sheet.Cell(row, 4).Value = transaction.ExpenseType.ToString();
                sheet.Cell(row, 5).Value = transaction.Amount?.Amount ?? 0m;
                sheet.Cell(row, 6).Value = transaction.Amount?.Currency.ToString() ?? "";
                row++;
            }

            ApplyHeaderStyle(sheet, "A3:F3");
            for (var currentRow = 4; currentRow <= row - 1; currentRow++)
            {
                sheet.Row(currentRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                sheet.Cell(currentRow, 1).Style.NumberFormat.Format = "dd/MM/yyyy";
                sheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
            }

            sheet.Columns().AdjustToContents();
            sheet.Columns("E", "E").Style.NumberFormat.Format = "#,##0.00";
            sheet.Column("A").Style.NumberFormat.Format = "dd/MM/yyyy";
        }

        private static void AddMonthlySheets(XLWorkbook workbook, IReadOnlyList<Transaction> transactions)
        {
            var groupedTransactions = transactions
                .GroupBy(transaction => new DateTime(transaction.Date.Year, transaction.Date.Month, 1))
                .OrderBy(group => group.Key);

            foreach (var group in groupedTransactions)
            {
                var sheet = workbook.Worksheets.Add(group.Key.ToString("yyyy-MM"));
                sheet.Cell("A1").Value = $"{group.Key:MMMM yyyy} report";
                sheet.Range("A1:F1").Merge();
                sheet.Cell("A1").Style.Font.SetBold();
                sheet.Cell("A1").Style.Font.FontSize = 14;
                sheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                sheet.Cell("A1").Style.Font.FontColor = XLColor.White;
                sheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                var groupTransactions = group.OrderBy(item => item.Date).ToList();
                var totalExpenses = groupTransactions.Where(t => t.ExpenseType == TransactionType.Expense).Sum(t => t.Amount?.Amount ?? 0m);
                var totalIncome = groupTransactions.Where(t => t.ExpenseType == TransactionType.Income).Sum(t => t.Amount?.Amount ?? 0m);
                var net = totalIncome - totalExpenses;

                sheet.Cell("A3").Value = "Transactions";
                sheet.Cell("B3").Value = groupTransactions.Count;
                sheet.Cell("D3").Value = "Income";
                sheet.Cell("E3").Value = totalIncome;
                sheet.Cell("A4").Value = "Expenses";
                sheet.Cell("B4").Value = totalExpenses;
                sheet.Cell("D4").Value = "Net";
                sheet.Cell("E4").Value = net;

                sheet.Cell("A6").Value = "Date";
                sheet.Cell("B6").Value = "Description";
                sheet.Cell("C6").Value = "Category";
                sheet.Cell("D6").Value = "Type";
                sheet.Cell("E6").Value = "Amount";
                sheet.Cell("F6").Value = "Currency";

                var row = 7;
                foreach (var transaction in groupTransactions)
                {
                    sheet.Cell(row, 1).Value = transaction.Date.Date;
                    sheet.Cell(row, 2).Value = transaction.Description ?? "";
                    sheet.Cell(row, 3).Value = transaction.Category?.Name ?? GetCategoryName(transaction.CategoryId);
                    sheet.Cell(row, 4).Value = transaction.ExpenseType.ToString();
                    sheet.Cell(row, 5).Value = transaction.Amount?.Amount ?? 0m;
                    sheet.Cell(row, 6).Value = transaction.Amount?.Currency.ToString() ?? "";
                    row++;
                }

                ApplyHeaderStyle(sheet, "A6:F6");
                for (var currentRow = 7; currentRow <= row - 1; currentRow++)
                {
                    sheet.Row(currentRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    sheet.Cell(currentRow, 1).Style.NumberFormat.Format = "dd/MM/yyyy";
                    sheet.Cell(currentRow, 5).Style.NumberFormat.Format = "#,##0.00";
                }

                sheet.Column("A").Style.NumberFormat.Format = "dd/MM/yyyy";
                sheet.Column("E").Style.NumberFormat.Format = "#,##0.00";
                sheet.Columns().AdjustToContents();
            }
        }

        private static void AddSummarySheet(XLWorkbook workbook, IReadOnlyList<Transaction> transactions, IReadOnlyList<Category> categories)
        {
            var summarySheet = workbook.Worksheets.Add("Summary");
            summarySheet.Cell("A1").Value = "Financial Overview";
            summarySheet.Range("A1:H1").Merge();
            summarySheet.Cell("A1").Style.Font.SetBold();
            summarySheet.Cell("A1").Style.Font.FontSize = 16;
            summarySheet.Cell("A1").Style.Fill.BackgroundColor = XLColor.FromHtml("#D9EAF7");
            summarySheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var totalTransactions = transactions.Count;
            var totalIncome = transactions.Where(t => t.ExpenseType == TransactionType.Income).Sum(t => t.Amount?.Amount ?? 0m);
            var totalExpenses = transactions.Where(t => t.ExpenseType == TransactionType.Expense).Sum(t => t.Amount?.Amount ?? 0m);
            var net = totalIncome - totalExpenses;
            var topCategory = categories
                .Select(category => new
                {
                    category.Name,
                    Total = transactions.Where(t => t.CategoryId == category.Id).Sum(t => t.Amount?.Amount ?? 0m)
                })
                .OrderByDescending(item => item.Total)
                .FirstOrDefault();

            var expenseTransactions = transactions
                .Where(t => t.ExpenseType == TransactionType.Expense)
                .ToList();

            var monthlyTotals = transactions
                .GroupBy(transaction => new DateTime(transaction.Date.Year, transaction.Date.Month, 1))
                .OrderBy(group => group.Key)
                .Select(group => new
                {
                    Period = group.Key,
                    Expenses = group.Where(t => t.ExpenseType == TransactionType.Expense).Sum(t => t.Amount?.Amount ?? 0m),
                    Income = group.Where(t => t.ExpenseType == TransactionType.Income).Sum(t => t.Amount?.Amount ?? 0m),
                    Net = group.Where(t => t.ExpenseType == TransactionType.Income).Sum(t => t.Amount?.Amount ?? 0m) -
                          group.Where(t => t.ExpenseType == TransactionType.Expense).Sum(t => t.Amount?.Amount ?? 0m)
                })
                .ToList();

            var monthlyExpenseValues = monthlyTotals.Select(x => x.Expenses).ToList();
            var weeklyExpenseValues = expenseTransactions
                .GroupBy(t => GetWeekStart(t.Date))
                .Select(group => group.Sum(item => item.Amount?.Amount ?? 0m))
                .ToList();
            var dailyExpenseValues = expenseTransactions
                .GroupBy(t => t.Date.Date)
                .Select(group => group.Sum(item => item.Amount?.Amount ?? 0m))
                .ToList();

            var annualAverage = totalExpenses;
            var annualMedian = totalExpenses;
            var monthlyAverage = monthlyExpenseValues.Count == 0 ? 0m : monthlyExpenseValues.Average();
            var monthlyMedian = monthlyExpenseValues.Count == 0 ? 0m : GetMedian(monthlyExpenseValues);
            var weeklyAverage = weeklyExpenseValues.Count == 0 ? 0m : weeklyExpenseValues.Average();
            var weeklyMedian = weeklyExpenseValues.Count == 0 ? 0m : GetMedian(weeklyExpenseValues);
            var dailyAverage = dailyExpenseValues.Count == 0 ? 0m : dailyExpenseValues.Average();
            var dailyMedian = dailyExpenseValues.Count == 0 ? 0m : GetMedian(dailyExpenseValues);

            summarySheet.Cell("A4").Value = "Overview";
            summarySheet.Cell("A5").Value = "Total transactions";
            summarySheet.Cell("B5").Value = totalTransactions;
            summarySheet.Cell("A6").Value = "Income";
            summarySheet.Cell("B6").Value = totalIncome;
            summarySheet.Cell("A7").Value = "Expenses";
            summarySheet.Cell("B7").Value = totalExpenses;
            summarySheet.Cell("A8").Value = "Net";
            summarySheet.Cell("B8").Value = net;
            summarySheet.Cell("A9").Value = "Top category";
            summarySheet.Cell("B9").Value = topCategory?.Name ?? "N/A";

            summarySheet.Cell("A4").Style.Font.SetBold();
            summarySheet.Cell("A5").Value = "Total transactions";
            summarySheet.Cell("A7").Value = "Income";
            summarySheet.Cell("A8").Value = "Expenses";
            summarySheet.Cell("A9").Value = "Net";
            summarySheet.Cell("A10").Value = "Top category";

            summarySheet.Cell("A19").Value = "Statistiche spesa";
            summarySheet.Range("A19:E19").Merge();
            summarySheet.Cell("A19").Style.Font.SetBold();
            summarySheet.Cell("A19").Style.Fill.BackgroundColor = XLColor.FromHtml("#E2E8F0");
            summarySheet.Cell("A19").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            summarySheet.Cell("A20").Value = "Periodo";
            summarySheet.Cell("B20").Value = "Annuale";
            summarySheet.Cell("C20").Value = "Mensile";
            summarySheet.Cell("D20").Value = "Settimanale";
            summarySheet.Cell("E20").Value = "Giornaliera";

            summarySheet.Cell("A21").Value = "Media";
            summarySheet.Cell("B21").Value = annualAverage;
            summarySheet.Cell("C21").Value = monthlyAverage;
            summarySheet.Cell("D21").Value = weeklyAverage;
            summarySheet.Cell("E21").Value = dailyAverage;

            summarySheet.Cell("A22").Value = "Mediana";
            summarySheet.Cell("B22").Value = annualMedian;
            summarySheet.Cell("C22").Value = monthlyMedian;
            summarySheet.Cell("D22").Value = weeklyMedian;
            summarySheet.Cell("E22").Value = dailyMedian;

            summarySheet.Cell("A23").Value = "Distribuzione";
            summarySheet.Cell("B23").Value = 1m;
            summarySheet.Cell("C23").Value = totalExpenses == 0 ? 0m : monthlyAverage / totalExpenses;
            summarySheet.Cell("D23").Value = totalExpenses == 0 ? 0m : weeklyAverage / totalExpenses;
            summarySheet.Cell("E23").Value = totalExpenses == 0 ? 0m : dailyAverage / totalExpenses;

            ApplyHeaderStyle(summarySheet, "A19:E19");
            summarySheet.Range("A20:E22").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            summarySheet.Columns("B", "E").Style.NumberFormat.Format = "#,##0.00";
            summarySheet.Cell("B23").Style.NumberFormat.Format = "0.00%";
            summarySheet.Cell("C23").Style.NumberFormat.Format = "0.00%";
            summarySheet.Cell("D23").Style.NumberFormat.Format = "0.00%";
            summarySheet.Cell("E23").Style.NumberFormat.Format = "0.00%";

            summarySheet.Cell("D4").Value = "Month";
            summarySheet.Cell("E4").Value = "Expenses";
            summarySheet.Cell("F4").Value = "Income";
            summarySheet.Cell("G4").Value = "Net";

            var monthlyRow = 5;
            foreach (var item in monthlyTotals)
            {
                summarySheet.Cell(monthlyRow, 4).Value = item.Period.ToString("MMM yyyy");
                summarySheet.Cell(monthlyRow, 5).Value = item.Expenses;
                summarySheet.Cell(monthlyRow, 6).Value = item.Income;
                summarySheet.Cell(monthlyRow, 7).Value = item.Net;
                monthlyRow++;
            }

            ApplyHeaderStyle(summarySheet, "D4:G4");
            summarySheet.Range("D4:G100").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            summarySheet.Cell("J4").Value = "Category";
            summarySheet.Cell("K4").Value = "Total";
            summarySheet.Cell("L4").Value = "Share %";

            var totalExpensesForCategories = totalExpenses;
            var categoryRows = categories
                .OrderBy(c => c.Name)
                .Select(category => new
                {
                    Name = category.Name ?? "Unknown",
                    Total = transactions
                        .Where(t => t.CategoryId == category.Id && t.ExpenseType == TransactionType.Expense)
                        .Sum(t => t.Amount?.Amount ?? 0m)
                })
                .Where(item => item.Total > 0)
                .ToList();

            var categoryRow = 5;
            foreach (var category in categoryRows)
            {
                summarySheet.Cell(categoryRow, 10).Value = category.Name;
                summarySheet.Cell(categoryRow, 11).Value = category.Total;
                summarySheet.Cell(categoryRow, 12).Value = totalExpensesForCategories == 0 ? 0m : Math.Round((category.Total / totalExpensesForCategories) * 100m, 2);
                categoryRow++;
            }

            ApplyHeaderStyle(summarySheet, "J4:L4");

            summarySheet.Range("D5:D14").Style.NumberFormat.Format = "MMM yyyy";
            summarySheet.Columns("E", "G").Style.NumberFormat.Format = "#,##0.00";
            summarySheet.Columns("J", "L").Style.NumberFormat.Format = "#,##0.00";
            summarySheet.Range("D5:G100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            summarySheet.Columns().AdjustToContents();
        }

        private static Font? GetChartFont()
        {
            FontFamily? fontFamily = null;
            foreach (var family in SystemFonts.Collection.Families)
            {
                if (string.IsNullOrWhiteSpace(family.Name))
                {
                    continue;
                }

                if (family.Name.Contains("Arial", StringComparison.OrdinalIgnoreCase))
                {
                    fontFamily = family;
                    break;
                }

                fontFamily ??= family;
            }

            return fontFamily is not null ? new Font(fontFamily.Value, 12) : null;
        }

        private static DateTime GetWeekStart(DateTime date)
        {
            var day = date.DayOfWeek;
            var offset = day == DayOfWeek.Sunday ? 6 : (int)day - 1;
            return date.Date.AddDays(-offset);
        }

        private static decimal GetMedian(IReadOnlyCollection<decimal> values)
        {
            if (values.Count == 0)
            {
                return 0m;
            }

            var ordered = values.OrderBy(value => value).ToList();
            var middle = ordered.Count / 2;

            if (ordered.Count % 2 == 0)
            {
                return (ordered[middle - 1] + ordered[middle]) / 2m;
            }

            return ordered[middle];
        }

        private static void ApplyHeaderStyle(IXLWorksheet sheet, string range)
        {
            var headerRange = sheet.Range(range);
            var style = headerRange.Style;
            style.Font.Bold = true;
            style.Fill.BackgroundColor = XLColor.FromHtml("#D9EAF7");
            style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        private static string GetCategoryName(long categoryId)
        {
            return categoryId > 0 ? $"Category {categoryId}" : "Uncategorized";
        }
    }
}
