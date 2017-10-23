using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PharmaBook.Migrations
{
    public partial class newsales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChildInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BatchNo = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ExpDt = table.Column<string>(nullable: true),
                    MasterId_idnty = table.Column<int>(nullable: false),
                    MasterInvoiceId = table.Column<int>(nullable: true),
                    Mrg = table.Column<string>(nullable: true),
                    PrdId = table.Column<int>(nullable: false),
                    Qty = table.Column<int>(nullable: false),
                    Rs = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChildInvoice_MstrInvc_MasterInvoiceId",
                        column: x => x.MasterInvoiceId,
                        principalTable: "MstrInvc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChildInvoice_MasterInvoiceId",
                table: "ChildInvoice",
                column: "MasterInvoiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildInvoice");
        }
    }
}
