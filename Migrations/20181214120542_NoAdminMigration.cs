using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PRAPI.Migrations
{
    public partial class NoAdminMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "License",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "NextInsurancePayment",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "NextTechReview",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Cars");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "License",
                table: "Cars",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextInsurancePayment",
                table: "Cars",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "NextTechReview",
                table: "Cars",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Cars",
                nullable: true);
        }
    }
}
