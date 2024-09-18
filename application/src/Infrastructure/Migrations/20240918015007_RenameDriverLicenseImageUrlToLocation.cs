using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameDriverLicenseImageUrlToLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
            name: "DriverLicenseImageUrl",
            table: "UserProfiles",
            newName: "DriverLicenseImageLocation");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("fa885a78-582a-451e-972f-aa27ae7935a9"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { new Guid("eeedf23a-f819-4c39-9b70-1cbac95641ab"), new DateTime(2024, 9, 18, 1, 50, 7, 33, DateTimeKind.Utc).AddTicks(1556), "$2y$10$mWs/f1yA29KsATYQZeAjbu5VPP0bG/SJ.WX3qXjmqbbeaXUjPtrhO", 0, new DateTime(2024, 9, 18, 1, 50, 7, 33, DateTimeKind.Utc).AddTicks(1557), "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DriverLicenseImageLocation",
                table: "UserProfiles",
                newName: "DriverLicenseImageUrl");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eeedf23a-f819-4c39-9b70-1cbac95641ab"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { new Guid("fa885a78-582a-451e-972f-aa27ae7935a9"), new DateTime(2024, 9, 15, 22, 23, 22, 568, DateTimeKind.Utc).AddTicks(806), "$2y$10$mWs/f1yA29KsATYQZeAjbu5VPP0bG/SJ.WX3qXjmqbbeaXUjPtrhO", 0, new DateTime(2024, 9, 15, 22, 23, 22, 568, DateTimeKind.Utc).AddTicks(806), "admin" });
        }
    }
}
