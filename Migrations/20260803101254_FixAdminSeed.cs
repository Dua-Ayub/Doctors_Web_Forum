using Microsoft.EntityFrameworkCore.Migrations;



#nullable disable

namespace Doctors_Web_Forum.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "PasswordHash", "Username" },
                values: new object[] { 1, "AQAAAAIAAYagAAAAEHOSokIP8jU5w1y+Xrg11fxyW9osQNV2yrXblPJ2dPACgdWLLyydRmbPs3DANIer/g==", "admin" });
        }
    }
}
