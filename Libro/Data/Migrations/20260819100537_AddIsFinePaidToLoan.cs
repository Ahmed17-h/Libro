using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFinePaidToLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFinePaid",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFinePaid",
                table: "Loans");
        }
    }
}
