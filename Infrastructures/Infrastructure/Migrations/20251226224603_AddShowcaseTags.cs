using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShowcaseTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEditorsChoice",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNewRelease",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_IsEditorsChoice",
                table: "Movies",
                column: "IsEditorsChoice");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_IsNewRelease",
                table: "Movies",
                column: "IsNewRelease");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_IsEditorsChoice",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_IsNewRelease",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "IsEditorsChoice",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "IsNewRelease",
                table: "Movies");
        }
    }
}
