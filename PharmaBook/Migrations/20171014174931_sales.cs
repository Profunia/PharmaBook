using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PharmaBook.Migrations
{
    public partial class sales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "vendorID",
                table: "purchasedHistory",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "MstrInvc",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DrName = table.Column<string>(nullable: true),
                    InvCrtdate = table.Column<DateTime>(nullable: false),
                    InvId = table.Column<string>(nullable: true),
                    PatientAdres = table.Column<string>(nullable: true),
                    PatientName = table.Column<string>(nullable: true),
                    RegNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MstrInvc", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MstrInvc");

            migrationBuilder.AlterColumn<int>(
                name: "vendorID",
                table: "purchasedHistory",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
