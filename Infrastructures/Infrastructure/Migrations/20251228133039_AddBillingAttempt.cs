using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingAttempts_Members_MemberId",
                table: "BillingAttempts");

            migrationBuilder.DropIndex(
                name: "IX_BillingAttempts_MemberId_Period",
                table: "BillingAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                table: "BillingAttempts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7);

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "BillingAttempts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingAttempts_MemberId_Period",
                table: "BillingAttempts",
                columns: new[] { "MemberId", "Period" });

            migrationBuilder.AddForeignKey(
                name: "FK_BillingAttempts_Members_MemberId",
                table: "BillingAttempts",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillingAttempts_Members_MemberId",
                table: "BillingAttempts");

            migrationBuilder.DropIndex(
                name: "IX_BillingAttempts_MemberId_Period",
                table: "BillingAttempts");

            migrationBuilder.AlterColumn<string>(
                name: "Period",
                table: "BillingAttempts",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "BillingAttempts",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingAttempts_MemberId_Period",
                table: "BillingAttempts",
                columns: new[] { "MemberId", "Period" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BillingAttempts_Members_MemberId",
                table: "BillingAttempts",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
