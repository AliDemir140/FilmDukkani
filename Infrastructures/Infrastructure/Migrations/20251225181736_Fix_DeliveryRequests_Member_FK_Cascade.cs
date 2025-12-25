using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Fix_DeliveryRequests_Member_FK_Cascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // FK zaten varsa tekrar ekleme (DB'de isim çakışıyor)
            migrationBuilder.Sql(@"
IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_DeliveryRequests_Members_MemberId'
)
BEGIN
    ALTER TABLE [DeliveryRequests]
    ADD CONSTRAINT [FK_DeliveryRequests_Members_MemberId]
    FOREIGN KEY ([MemberId]) REFERENCES [Members]([ID]) ON DELETE NO ACTION;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // FK varsa sil (yoksa patlamasın)
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_DeliveryRequests_Members_MemberId'
)
BEGIN
    ALTER TABLE [DeliveryRequests]
    DROP CONSTRAINT [FK_DeliveryRequests_Members_MemberId];
END
");
        }
    }
}
