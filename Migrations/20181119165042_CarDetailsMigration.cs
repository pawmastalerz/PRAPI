using Microsoft.EntityFrameworkCore.Migrations;

namespace PRAPI.Migrations
{
    public partial class CarDetailsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Class",
                table: "Cars",
                newName: "Fuel");

            migrationBuilder.AddColumn<string>(
                name: "AirConditioned",
                table: "Cars",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "Cars",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AirConditioned",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Body",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "Fuel",
                table: "Cars",
                newName: "Class");
        }
    }
}
