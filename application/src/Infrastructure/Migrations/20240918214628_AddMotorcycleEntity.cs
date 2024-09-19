using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMotorcycleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eeedf23a-f819-4c39-9b70-1cbac95641ab"));

            migrationBuilder.CreateTable(
                name: "Motorcycles",
                columns: table => new
                {
                    MotorcyleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    LicensePlate = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motorcycles", x => x.MotorcyleId);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { new Guid("1384db6a-9368-4c4d-9fb8-43ed99c7cebd"), new DateTime(2024, 9, 18, 21, 46, 28, 404, DateTimeKind.Utc).AddTicks(167), "$2y$10$mWs/f1yA29KsATYQZeAjbu5VPP0bG/SJ.WX3qXjmqbbeaXUjPtrhO", 0, new DateTime(2024, 9, 18, 21, 46, 28, 404, DateTimeKind.Utc).AddTicks(167), "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Motorcycles_LicensePlate",
                table: "Motorcycles",
                column: "LicensePlate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Motorcycles");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1384db6a-9368-4c4d-9fb8-43ed99c7cebd"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { new Guid("eeedf23a-f819-4c39-9b70-1cbac95641ab"), new DateTime(2024, 9, 18, 1, 50, 7, 33, DateTimeKind.Utc).AddTicks(1556), "$2y$10$mWs/f1yA29KsATYQZeAjbu5VPP0bG/SJ.WX3qXjmqbbeaXUjPtrhO", 0, new DateTime(2024, 9, 18, 1, 50, 7, 33, DateTimeKind.Utc).AddTicks(1557), "admin" });
        }
    }
}
