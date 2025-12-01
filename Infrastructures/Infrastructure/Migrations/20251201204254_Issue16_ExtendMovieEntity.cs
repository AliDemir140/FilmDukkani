using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Issue16_ExtendMovieEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Movies",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<string>(
                name: "AudioFeatures",
                table: "Movies",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Movies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Movies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalTitle",
                table: "Movies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubtitleLanguages",
                table: "Movies",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "Movies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechnicalDetails",
                table: "Movies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrailerUrl",
                table: "Movies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioFeatures",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "OriginalTitle",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "SubtitleLanguages",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "TechnicalDetails",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "TrailerUrl",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Movies",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
