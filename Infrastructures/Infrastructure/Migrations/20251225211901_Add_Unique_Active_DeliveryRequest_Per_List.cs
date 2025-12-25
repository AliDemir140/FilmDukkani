using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Unique_Active_DeliveryRequest_Per_List : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeliveryRequests_MemberMovieListId",
                table: "DeliveryRequests");

            migrationBuilder.CreateIndex(
                name: "UX_DeliveryRequests_ActivePerList",
                table: "DeliveryRequests",
                column: "MemberMovieListId",
                unique: true,
                filter: "[Status] IN (0,1,2,3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_DeliveryRequests_ActivePerList",
                table: "DeliveryRequests");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryRequests_MemberMovieListId",
                table: "DeliveryRequests",
                column: "MemberMovieListId");
        }
    }
}
