using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.ID);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 8, 16, 28, 23, 525, DateTimeKind.Local).AddTicks(6605), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4064) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 6, 4, 36, 10, 807, DateTimeKind.Local).AddTicks(8000), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4069) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 5, 9, 3, 0, 6, 821, DateTimeKind.Local).AddTicks(5601), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4071) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 17, 20, 3, 14, 741, DateTimeKind.Local).AddTicks(4038), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4073) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 4, 22, 12, 18, 25, 443, DateTimeKind.Local).AddTicks(9012), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4075) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 3, 31, 17, 11, 5, 165, DateTimeKind.Local).AddTicks(6432), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4077) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 28, 3, 54, 1, 155, DateTimeKind.Local).AddTicks(8197), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4079) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 6, 6, 8, 48, 20, 762, DateTimeKind.Local).AddTicks(7973), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4080) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 5, 20, 6, 14, 43, 881, DateTimeKind.Local).AddTicks(4644), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4082) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 21, 14, 7, 56, 346, DateTimeKind.Local).AddTicks(8681), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4084) });
        }
    }
}
