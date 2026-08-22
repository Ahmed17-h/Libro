using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRenewedToLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRenewed",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRenewed",
                table: "Loans");
        }
    }
}
