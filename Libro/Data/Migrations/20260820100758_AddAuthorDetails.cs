using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libro.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Authors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Authors");
        }
    }
}
