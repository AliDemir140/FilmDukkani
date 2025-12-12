using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieCopyIdToDeliveryRequestItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovieCopyId",
                table: "DeliveryRequestItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRequestItems_MovieCopyId",
                table: "DeliveryRequestItems",
                column: "MovieCopyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryRequestItems_MovieCopies_MovieCopyId",
                table: "DeliveryRequestItems",
                column: "MovieCopyId",
                principalTable: "MovieCopies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryRequestItems_MovieCopies_MovieCopyId",
                table: "DeliveryRequestItems");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryRequestItems_MovieCopyId",
                table: "DeliveryRequestItems");

            migrationBuilder.DropColumn(
                name: "MovieCopyId",
                table: "DeliveryRequestItems");
        }
    }
}
