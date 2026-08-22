using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBookBorrowPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BorrowPrice",
                table: "Books",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowPrice",
                table: "Books");
        }
    }
}
