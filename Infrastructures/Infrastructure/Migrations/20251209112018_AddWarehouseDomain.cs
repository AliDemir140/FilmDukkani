using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MovieDirectors_MovieId",
                table: "MovieDirectors");

            migrationBuilder.DropIndex(
                name: "IX_MovieAwards_MovieId",
                table: "MovieAwards");

            migrationBuilder.DropIndex(
                name: "IX_MovieActors_MovieId",
                table: "MovieActors");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWinner",
                table: "MovieAwards",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "Shelves",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shelves", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MovieCopies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShelfId = table.Column<int>(type: "int", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDamaged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieCopies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MovieCopies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovieCopies_Shelves_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "Shelves",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DamagedMovies",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieCopyId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSentToPurchase = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamagedMovies", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DamagedMovies_MovieCopies_MovieCopyId",
                        column: x => x.MovieCopyId,
                        principalTable: "MovieCopies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieDirectors_MovieId_DirectorId",
                table: "MovieDirectors",
                columns: new[] { "MovieId", "DirectorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieAwards_MovieId_AwardId_Year",
                table: "MovieAwards",
                columns: new[] { "MovieId", "AwardId", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_MovieActors_MovieId_ActorId",
                table: "MovieActors",
                columns: new[] { "MovieId", "ActorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Directors_FirstName_LastName",
                table: "Directors",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Actors_FirstName_LastName",
                table: "Actors",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_DamagedMovies_MovieCopyId",
                table: "DamagedMovies",
                column: "MovieCopyId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCopies_Barcode",
                table: "MovieCopies",
                column: "Barcode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieCopies_MovieId",
                table: "MovieCopies",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieCopies_ShelfId",
                table: "MovieCopies",
                column: "ShelfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DamagedMovies");

            migrationBuilder.DropTable(
                name: "MovieCopies");

            migrationBuilder.DropTable(
                name: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_MovieDirectors_MovieId_DirectorId",
                table: "MovieDirectors");

            migrationBuilder.DropIndex(
                name: "IX_MovieAwards_MovieId_AwardId_Year",
                table: "MovieAwards");

            migrationBuilder.DropIndex(
                name: "IX_MovieActors_MovieId_ActorId",
                table: "MovieActors");

            migrationBuilder.DropIndex(
                name: "IX_Directors_FirstName_LastName",
                table: "Directors");

            migrationBuilder.DropIndex(
                name: "IX_Actors_FirstName_LastName",
                table: "Actors");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWinner",
                table: "MovieAwards",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_MovieDirectors_MovieId",
                table: "MovieDirectors",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieAwards_MovieId",
                table: "MovieAwards",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieActors_MovieId",
                table: "MovieActors",
                column: "MovieId");
        }
    }
}
