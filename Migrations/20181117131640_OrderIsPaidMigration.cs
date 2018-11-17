using Microsoft.EntityFrameworkCore.Migrations;

namespace PRAPI.Migrations
{
    public partial class OrderIsPaidMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsPaid",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Orders");
        }
    }
}
