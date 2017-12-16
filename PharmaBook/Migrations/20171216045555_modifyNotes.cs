using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PharmaBook.Migrations
{
    public partial class modifyNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "createdDate",
                table: "Notes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "userName",
                table: "Notes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdDate",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "userName",
                table: "Notes");
        }
    }
}
