using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expenses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProxyFieldToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProxyTransaction",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProxyTransaction",
                table: "Transactions");
        }
    }
}
