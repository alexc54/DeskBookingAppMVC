using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeskBookingApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddDeskDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Desks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Desks");
        }
    }
}
