using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix_List_Delete_Cascade_DeliveryRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryRequests_Members_MemberId",
                table: "DeliveryRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryRequests_Members_MemberId",
                table: "DeliveryRequests",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryRequests_Members_MemberId",
                table: "DeliveryRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryRequests_Members_MemberId",
                table: "DeliveryRequests",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
