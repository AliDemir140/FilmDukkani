using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Fix_DeliveryRequests_List_FK_Cascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DeliveryRequests -> MemberMovieLists FK'yi düşür
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryRequests_MemberMovieLists_MemberMovieListId",
                table: "DeliveryRequests");

            // Cascade olarak tekrar ekle
            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryRequests_MemberMovieLists_MemberMovieListId",
                table: "DeliveryRequests",
                column: "MemberMovieListId",
                principalTable: "MemberMovieLists",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryRequests_MemberMovieLists_MemberMovieListId",
                table: "DeliveryRequests");

            // Eski haline (NoAction) geri dön
            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryRequests_MemberMovieLists_MemberMovieListId",
                table: "DeliveryRequests",
                column: "MemberMovieListId",
                principalTable: "MemberMovieLists",
                principalColumn: "ID",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
