using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PharmaBook.Migrations
{
    public partial class modifiedStef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "eachStefPrice",
                table: "purchasedHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "stef",
                table: "purchasedHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tabletsCapsule",
                table: "purchasedHistory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "eachStefPrice",
                table: "products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "stef",
                table: "products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tabletsCapsule",
                table: "products",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "eachStefPrice",
                table: "ChildPO",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "stef",
                table: "ChildPO",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tabletsCapsule",
                table: "ChildPO",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "eachStefPrice",
                table: "purchasedHistory");

            migrationBuilder.DropColumn(
                name: "stef",
                table: "purchasedHistory");

            migrationBuilder.DropColumn(
                name: "tabletsCapsule",
                table: "purchasedHistory");

            migrationBuilder.DropColumn(
                name: "eachStefPrice",
                table: "products");

            migrationBuilder.DropColumn(
                name: "stef",
                table: "products");

            migrationBuilder.DropColumn(
                name: "tabletsCapsule",
                table: "products");

            migrationBuilder.DropColumn(
                name: "eachStefPrice",
                table: "ChildPO");

            migrationBuilder.DropColumn(
                name: "stef",
                table: "ChildPO");

            migrationBuilder.DropColumn(
                name: "tabletsCapsule",
                table: "ChildPO");
        }
    }
}
