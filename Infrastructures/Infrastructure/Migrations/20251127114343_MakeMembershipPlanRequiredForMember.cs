using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeMembershipPlanRequiredForMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MembershipPlanId",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "MembershipStartDate",
                table: "Members",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 9, 19, 43, 13, 684, DateTimeKind.Local).AddTicks(1929), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9388) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 7, 7, 51, 0, 966, DateTimeKind.Local).AddTicks(3323), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9392) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 5, 10, 6, 14, 56, 980, DateTimeKind.Local).AddTicks(925), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9395) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 18, 23, 18, 4, 899, DateTimeKind.Local).AddTicks(9362), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9396) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 4, 23, 15, 33, 15, 602, DateTimeKind.Local).AddTicks(4335), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9398) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 4, 1, 20, 25, 55, 324, DateTimeKind.Local).AddTicks(1756), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9401) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 29, 7, 8, 51, 314, DateTimeKind.Local).AddTicks(3520), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9402) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 6, 7, 12, 3, 10, 921, DateTimeKind.Local).AddTicks(3297), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9404) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 5, 21, 9, 29, 34, 39, DateTimeKind.Local).AddTicks(9968), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9406) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 22, 17, 22, 46, 505, DateTimeKind.Local).AddTicks(4005), new DateTime(2025, 11, 27, 14, 43, 43, 291, DateTimeKind.Local).AddTicks(9408) });

            migrationBuilder.CreateIndex(
                name: "IX_Members_MembershipPlanId",
                table: "Members",
                column: "MembershipPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MembershipPlans_MembershipPlanId",
                table: "Members",
                column: "MembershipPlanId",
                principalTable: "MembershipPlans",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MembershipPlans_MembershipPlanId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_MembershipPlanId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MembershipPlanId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MembershipStartDate",
                table: "Members");

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
    }
}
