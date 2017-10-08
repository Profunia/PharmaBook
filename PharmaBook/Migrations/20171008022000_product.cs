using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PharmaBook.Migrations
{
    public partial class product : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MRP = table.Column<double>(nullable: true),
                    batchNo = table.Column<string>(nullable: true),
                    companyName = table.Column<string>(nullable: true),
                    cusUserName = table.Column<string>(nullable: true),
                    expDate = table.Column<DateTime>(nullable: false),
                    lastUpdated = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    openingStock = table.Column<double>(nullable: true),
                    stef = table.Column<string>(nullable: true),
                    tabletsPerStef = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");
        }
    }
}
