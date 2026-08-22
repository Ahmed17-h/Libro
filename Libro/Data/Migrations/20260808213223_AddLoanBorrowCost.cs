using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanBorrowCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BorrowCost",
                table: "Loans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowCost",
                table: "Loans");
        }
    }
}
