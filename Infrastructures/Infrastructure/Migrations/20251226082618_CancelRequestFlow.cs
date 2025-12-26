using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CancelRequestFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CancelApproved",
                table: "DeliveryRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDecisionAt",
                table: "DeliveryRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CancelPreviousStatus",
                table: "DeliveryRequests",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "DeliveryRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelRequestedAt",
                table: "DeliveryRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelApproved",
                table: "DeliveryRequests");

            migrationBuilder.DropColumn(
                name: "CancelDecisionAt",
                table: "DeliveryRequests");

            migrationBuilder.DropColumn(
                name: "CancelPreviousStatus",
                table: "DeliveryRequests");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "DeliveryRequests");

            migrationBuilder.DropColumn(
                name: "CancelRequestedAt",
                table: "DeliveryRequests");
        }
    }
}
