using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Movies_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "ID", "CategoryName", "CreatedDate", "ModifiedDate" },
                values: new object[,]
                {
                    { 1, "Aksiyon", new DateTime(2025, 10, 8, 16, 28, 23, 525, DateTimeKind.Local).AddTicks(6605), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4064) },
                    { 2, "Macera", new DateTime(2025, 10, 6, 4, 36, 10, 807, DateTimeKind.Local).AddTicks(8000), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4069) },
                    { 3, "Dram", new DateTime(2024, 5, 9, 3, 0, 6, 821, DateTimeKind.Local).AddTicks(5601), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4071) },
                    { 4, "Korku", new DateTime(2024, 11, 17, 20, 3, 14, 741, DateTimeKind.Local).AddTicks(4038), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4073) },
                    { 5, "Bilim Kurgu", new DateTime(2024, 4, 22, 12, 18, 25, 443, DateTimeKind.Local).AddTicks(9012), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4075) },
                    { 6, "Komedi", new DateTime(2024, 3, 31, 17, 11, 5, 165, DateTimeKind.Local).AddTicks(6432), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4077) },
                    { 7, "Gerilim", new DateTime(2025, 7, 28, 3, 54, 1, 155, DateTimeKind.Local).AddTicks(8197), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4079) },
                    { 8, "Fantastik", new DateTime(2024, 6, 6, 8, 48, 20, 762, DateTimeKind.Local).AddTicks(7973), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4080) },
                    { 9, "Belgesel", new DateTime(2025, 5, 20, 6, 14, 43, 881, DateTimeKind.Local).AddTicks(4644), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4082) },
                    { 10, "Animasyon", new DateTime(2024, 11, 21, 14, 7, 56, 346, DateTimeKind.Local).AddTicks(8681), new DateTime(2025, 11, 26, 11, 28, 53, 133, DateTimeKind.Local).AddTicks(4084) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CategoryId",
                table: "Movies",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
