using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doctors_Web_Forum.Migrations
{
    /// <inheritdoc />
    public partial class RenameBodyToDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Body",
                table: "Questions",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Questions",
                newName: "Body");
        }
    }
}
