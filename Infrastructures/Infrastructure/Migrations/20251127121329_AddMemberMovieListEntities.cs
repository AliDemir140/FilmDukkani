using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberMovieListEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberMovieLists",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberMovieLists", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MemberMovieLists_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberMovieListItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberMovieListId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberMovieListItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MemberMovieListItems_MemberMovieLists_MemberMovieListId",
                        column: x => x.MemberMovieListId,
                        principalTable: "MemberMovieLists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberMovieListItems_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 9, 20, 12, 59, 638, DateTimeKind.Local).AddTicks(118), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7585) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 10, 7, 8, 20, 46, 920, DateTimeKind.Local).AddTicks(1521), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7590) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 5, 10, 6, 44, 42, 933, DateTimeKind.Local).AddTicks(9122), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7591) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 4,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 18, 23, 47, 50, 853, DateTimeKind.Local).AddTicks(7558), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7593) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 5,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 4, 23, 16, 3, 1, 556, DateTimeKind.Local).AddTicks(2532), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7595) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 6,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 4, 1, 20, 55, 41, 277, DateTimeKind.Local).AddTicks(9953), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7598) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 7,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 7, 29, 7, 38, 37, 268, DateTimeKind.Local).AddTicks(1718), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7599) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 8,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 6, 7, 12, 32, 56, 875, DateTimeKind.Local).AddTicks(1494), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7601) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 9,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2025, 5, 21, 9, 59, 19, 993, DateTimeKind.Local).AddTicks(8165), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7603) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "ID",
                keyValue: 10,
                columns: new[] { "CreatedDate", "ModifiedDate" },
                values: new object[] { new DateTime(2024, 11, 22, 17, 52, 32, 459, DateTimeKind.Local).AddTicks(2202), new DateTime(2025, 11, 27, 15, 13, 29, 245, DateTimeKind.Local).AddTicks(7605) });

            migrationBuilder.CreateIndex(
                name: "IX_MemberMovieListItems_MemberMovieListId",
                table: "MemberMovieListItems",
                column: "MemberMovieListId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMovieListItems_MovieId",
                table: "MemberMovieListItems",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMovieLists_MemberId",
                table: "MemberMovieLists",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberMovieListItems");

            migrationBuilder.DropTable(
                name: "MemberMovieLists");

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
        }
    }
}
