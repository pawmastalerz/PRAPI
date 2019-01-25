using Microsoft.EntityFrameworkCore.Migrations;

namespace PRAPI.Migrations
{
    public partial class Lp100KmAsStringMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LP100Km",
                table: "Cars",
                nullable: true,
                oldClrType: typeof(float));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "LP100Km",
                table: "Cars",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
