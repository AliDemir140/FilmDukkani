using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Unique_MemberId_ListName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberMovieLists_MemberId",
                table: "MemberMovieLists");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMovieLists_MemberId_Name",
                table: "MemberMovieLists",
                columns: new[] { "MemberId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MemberMovieLists_MemberId_Name",
                table: "MemberMovieLists");

            migrationBuilder.CreateIndex(
                name: "IX_MemberMovieLists_MemberId",
                table: "MemberMovieLists",
                column: "MemberId");
        }
    }
}
