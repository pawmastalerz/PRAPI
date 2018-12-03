using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PRAPI.Migrations
{
    public partial class ReservationMovedToOrderMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedFrom",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "ReservedTo",
                table: "Cars");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedFrom",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedTo",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedFrom",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReservedTo",
                table: "Orders");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedFrom",
                table: "Cars",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedTo",
                table: "Cars",
                nullable: true);
        }
    }
}
