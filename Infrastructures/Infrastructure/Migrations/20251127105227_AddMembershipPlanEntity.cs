using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipPlanEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembershipPlans",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxMoviesPerMonth = table.Column<int>(type: "int", nullable: false),
                    MaxChangePerMonth = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipPlans", x => x.ID);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 9, 18, 51, 57, 69, DateTimeKind.Local).AddTicks(4190), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1650) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 7, 6, 59, 44, 351, DateTimeKind.Local).AddTicks(5585), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1654) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 5, 10, 5, 23, 40, 365, DateTimeKind.Local).AddTicks(3186), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1656) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 18, 22, 26, 48, 285, DateTimeKind.Local).AddTicks(1623), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1658) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 4, 23, 14, 41, 58, 987, DateTimeKind.Local).AddTicks(6597), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1659) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 4, 1, 19, 34, 38, 709, DateTimeKind.Local).AddTicks(4017), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1662) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 29, 6, 17, 34, 699, DateTimeKind.Local).AddTicks(5782), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1663) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 6, 7, 11, 11, 54, 306, DateTimeKind.Local).AddTicks(5558), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1665) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 5, 21, 8, 38, 17, 425, DateTimeKind.Local).AddTicks(2229), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1666) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 22, 16, 31, 29, 890, DateTimeKind.Local).AddTicks(6266), new DateTime(2025, 11, 27, 13, 52, 26, 677, DateTimeKind.Local).AddTicks(1669) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembershipPlans");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 8, 16, 38, 44, 329, DateTimeKind.Local).AddTicks(4029), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1492) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 6, 4, 46, 31, 611, DateTimeKind.Local).AddTicks(5428), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1497) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 5, 9, 3, 10, 27, 625, DateTimeKind.Local).AddTicks(3029), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1499) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 17, 20, 13, 35, 545, DateTimeKind.Local).AddTicks(1466), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1501) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 4, 22, 12, 28, 46, 247, DateTimeKind.Local).AddTicks(6440), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1503) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 3, 31, 17, 21, 25, 969, DateTimeKind.Local).AddTicks(3861), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1505) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 28, 4, 4, 21, 959, DateTimeKind.Local).AddTicks(5625), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1507) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 6, 6, 8, 58, 41, 566, DateTimeKind.Local).AddTicks(5402), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1509) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 5, 20, 6, 25, 4, 685, DateTimeKind.Local).AddTicks(2073), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1510) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 21, 14, 18, 17, 150, DateTimeKind.Local).AddTicks(6110), new DateTime(2025, 11, 26, 11, 39, 13, 937, DateTimeKind.Local).AddTicks(1513) });
        }
    }
}
