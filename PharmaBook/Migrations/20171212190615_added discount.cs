using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PharmaBook.Migrations
{
    public partial class addeddiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "InvMaster",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "InvMaster");
        }
    }
}
