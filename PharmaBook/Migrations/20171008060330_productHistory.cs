using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PharmaBook.Migrations
{
    public partial class productHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "stef",
                table: "products");

            migrationBuilder.DropColumn(
                name: "tabletsPerStef",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "vedorName",
                table: "vendors",
                newName: "vendorName");

            migrationBuilder.RenameColumn(
                name: "vedorMobile",
                table: "vendors",
                newName: "vendorMobile");

            migrationBuilder.RenameColumn(
                name: "vedorCompnay",
                table: "vendors",
                newName: "vendorCompnay");

            migrationBuilder.RenameColumn(
                name: "vedorAddress",
                table: "vendors",
                newName: "vendorAddress");

            migrationBuilder.AlterColumn<int>(
                name: "openingStock",
                table: "products",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MRP",
                table: "products",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "vendorID",
                table: "products",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "purchasedHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MRP = table.Column<string>(nullable: true),
                    ProductID = table.Column<int>(nullable: false),
                    cusUserName = table.Column<string>(nullable: true),
                    purchasedDated = table.Column<DateTime>(nullable: false),
                    qty = table.Column<string>(nullable: true),
                    vendorID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchasedHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchasedHistory");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "products");

            migrationBuilder.DropColumn(
                name: "vendorID",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "vendorName",
                table: "vendors",
                newName: "vedorName");

            migrationBuilder.RenameColumn(
                name: "vendorMobile",
                table: "vendors",
                newName: "vedorMobile");

            migrationBuilder.RenameColumn(
                name: "vendorCompnay",
                table: "vendors",
                newName: "vedorCompnay");

            migrationBuilder.RenameColumn(
                name: "vendorAddress",
                table: "vendors",
                newName: "vedorAddress");

            migrationBuilder.AlterColumn<double>(
                name: "openingStock",
                table: "products",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<double>(
                name: "MRP",
                table: "products",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "stef",
                table: "products",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tabletsPerStef",
                table: "products",
                nullable: true);
        }
    }
}
