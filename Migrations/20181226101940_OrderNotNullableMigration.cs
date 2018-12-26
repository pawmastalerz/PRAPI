using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PRAPI.Migrations
{
    public partial class OrderNotNullableMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservedTo",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservedFrom",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Orders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservedTo",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservedFrom",
                table: "Orders",
                nullable: true,
                oldClrType: typeof(DateTime));
        }
    }
}
