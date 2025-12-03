using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Issue17_DeliveryRequestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryRequests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    MemberMovieListId = table.Column<int>(type: "int", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeliveryRequests_MemberMovieLists_MemberMovieListId",
                        column: x => x.MemberMovieListId,
                        principalTable: "MemberMovieLists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryRequests_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryRequestItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryRequestId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    MemberMovieListItemId = table.Column<int>(type: "int", nullable: false),
                    IsReturned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDamaged = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryRequestItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DeliveryRequestItems_DeliveryRequests_DeliveryRequestId",
                        column: x => x.DeliveryRequestId,
                        principalTable: "DeliveryRequests",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryRequestItems_MemberMovieListItems_MemberMovieListItemId",
                        column: x => x.MemberMovieListItemId,
                        principalTable: "MemberMovieListItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryRequestItems_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRequestItems_DeliveryRequestId",
                table: "DeliveryRequestItems",
                column: "DeliveryRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRequestItems_MemberMovieListItemId",
                table: "DeliveryRequestItems",
                column: "MemberMovieListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRequestItems_MovieId",
                table: "DeliveryRequestItems",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRequests_MemberId",
                table: "DeliveryRequests",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRequests_MemberMovieListId",
                table: "DeliveryRequests",
                column: "MemberMovieListId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryRequestItems");

            migrationBuilder.DropTable(
                name: "DeliveryRequests");
        }
    }
}
