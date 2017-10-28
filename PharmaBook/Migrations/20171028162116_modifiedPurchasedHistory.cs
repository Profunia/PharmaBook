using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PharmaBook.Migrations
{
    public partial class modifiedPurchasedHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BatchNo",
                table: "purchasedHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpDate",
                table: "purchasedHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mfg",
                table: "purchasedHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "purchasedHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "purchasedHistory",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BatchNo",
                table: "purchasedHistory");

            migrationBuilder.DropColumn(
                name: "ExpDate",
                table: "purchasedHistory");

            migrationBuilder.DropColumn(
                name: "Mfg",
                table: "purchasedHistory");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "purchasedHistory");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "purchasedHistory");
        }
    }
}
