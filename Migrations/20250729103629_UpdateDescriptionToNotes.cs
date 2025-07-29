using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeskBookingApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDescriptionToNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Desks",
                newName: "Notes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Desks",
                newName: "Description");
        }
    }
}
