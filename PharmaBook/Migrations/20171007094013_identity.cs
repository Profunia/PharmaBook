using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PharmaBook.Migrations
{
    public partial class identity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CusId",
                table: "vendors");

            migrationBuilder.AddColumn<string>(
                name: "cusUserName",
                table: "vendors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cusUserName",
                table: "vendors");

            migrationBuilder.AddColumn<int>(
                name: "CusId",
                table: "vendors",
                nullable: false,
                defaultValue: 0);
        }
    }
}
