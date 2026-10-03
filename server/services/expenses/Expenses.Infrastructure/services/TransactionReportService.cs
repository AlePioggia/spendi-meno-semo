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

            summarySheet.Cell("A4").Value = "Overview";
            summarySheet.Cell("A5").Value = "Total transactions";
            summarySheet.Cell("B5").Value = totalTransactions;
            summarySheet.Cell("A6").Value = "Monthly Totals";
            summarySheet.Cell("A7").Value = "Income";
            summarySheet.Cell("B7").Value = totalIncome;
            summarySheet.Cell("A8").Value = "Expenses";
            summarySheet.Cell("B8").Value = totalExpenses;
            summarySheet.Cell("A9").Value = "Net";
            summarySheet.Cell("B9").Value = net;
            summarySheet.Cell("A10").Value = "Top category";
            summarySheet.Cell("B10").Value = topCategory?.Name ?? "N/A";

            summarySheet.Cell("A4").Style.Font.SetBold();
            summarySheet.Cell("A5").Value = "Total transactions";
            summarySheet.Cell("A7").Value = "Income";
            summarySheet.Cell("A8").Value = "Expenses";
            summarySheet.Cell("A9").Value = "Net";
            summarySheet.Cell("A10").Value = "Top category";

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
            summarySheet.Cell("E6").Value = "Category Totals";

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

            var monthlyChartBytes = CreateBarChartImage(
                monthlyTotals.Select(item => item.Period.ToString("MMM")).ToList(),
                monthlyTotals.Select(item => item.Expenses).ToList(),
                "Expenses by month",
                new[] { "Expenses" });

            var monthlyPicture = summarySheet.AddPicture(new MemoryStream(monthlyChartBytes), XLPictureFormat.Png, "MonthlyTotalsChart");
            monthlyPicture.MoveTo(summarySheet.Cell("D20"));
            monthlyPicture.Scale(0.9);

            var categoryChartBytes = CreateBarChartImage(
                categoryRows.Select(item => item.Name).ToList(),
                categoryRows.Select(item => item.Total).ToList(),
                "Expense share by category",
                new[] { "Categories" });

            var categoryPicture = summarySheet.AddPicture(new MemoryStream(categoryChartBytes), XLPictureFormat.Png, "CategoryTotalsChart");
            categoryPicture.MoveTo(summarySheet.Cell("J20"));
            categoryPicture.Scale(0.9);

            summarySheet.Column("D").Style.NumberFormat.Format = "MMM yyyy";
            summarySheet.Columns("E", "G").Style.NumberFormat.Format = "#,##0.00";
            summarySheet.Columns("J", "L").Style.NumberFormat.Format = "#,##0.00";
            summarySheet.Range("D5:G100").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            summarySheet.Columns().AdjustToContents();
        }

        private static byte[] CreateBarChartImage(IReadOnlyList<string> labels, IReadOnlyList<decimal> values, string title, IReadOnlyList<string> legendLabels)
        {
            if (labels.Count == 0 || values.Count == 0)
            {
                using var empty = new Image<Rgba32>(420, 220);
                empty.Mutate(ctx => ctx.BackgroundColor(Color.White));
                using var emptyStream = new MemoryStream();
                empty.SaveAsPng(emptyStream);
                return emptyStream.ToArray();
            }

            const int width = 420;
            const int height = 220;
            var image = new Image<Rgba32>(width, height);
            var colors = new[]
            {
                Color.FromRgb(31, 119, 180),
                Color.FromRgb(255, 127, 14),
                Color.FromRgb(44, 160, 44),
                Color.FromRgb(214, 39, 40),
                Color.FromRgb(148, 103, 189),
                Color.FromRgb(140, 86, 75)
            };

            image.Mutate(ctx =>
            {
                ctx.BackgroundColor(Color.White);

                var maxValue = values.Max();
                var plotLeft = 30f;
                var plotTop = 25f;
                var plotWidth = width - 80f;
                var plotHeight = height - 70f;
                var axisColor = Color.DarkGray;

                ctx.Fill(axisColor, new RectangleF(plotLeft, plotTop + plotHeight, plotWidth, 2f));
                ctx.Fill(axisColor, new RectangleF(plotLeft, plotTop, 2f, plotHeight));

                var barStep = plotWidth / Math.Max(labels.Count, 1);
                var barWidth = Math.Max(16f, (float)(barStep * 0.6));

                for (var index = 0; index < labels.Count; index++)
                {
                    var value = values[index];
                    var relativeHeight = maxValue <= 0 ? 0f : (float)(value / maxValue);
                    var barHeight = relativeHeight * (plotHeight - 10);
                    var x = plotLeft + index * barStep + ((barStep - barWidth) / 2f);
                    var y = plotTop + plotHeight - barHeight;

                    ctx.Fill(colors[index % colors.Length], new RectangleF(x, y, barWidth, barHeight));
                }

                var chartFont = GetChartFont();
                if (chartFont is not null)
                {
                    ctx.DrawText(title, chartFont, Color.Black, new PointF(30f, 5f));

                    var legendX = 300f;
                    var legendY = 10f;
                    for (var index = 0; index < legendLabels.Count; index++)
                    {
                        ctx.Fill(colors[index % colors.Length], new RectangleF(legendX, legendY + (index * 16), 10f, 10f));
                        ctx.DrawText(legendLabels[index], chartFont, Color.Black, new PointF(legendX + 14f, legendY + (index * 16) - 2f));
                    }
                }
            });

            using var stream = new MemoryStream();
            image.SaveAsPng(stream);
            return stream.ToArray();
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
