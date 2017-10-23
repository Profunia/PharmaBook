using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PharmaBook.Migrations
{
    public partial class modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvMaster_InvChild_ChildInvoiceId",
                table: "InvMaster");

            migrationBuilder.DropIndex(
                name: "IX_InvMaster_ChildInvoiceId",
                table: "InvMaster");

            migrationBuilder.DropColumn(
                name: "ChildInvoiceId",
                table: "InvMaster");

            migrationBuilder.AddColumn<int>(
                name: "MasterInvID",
                table: "InvChild",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterInvID",
                table: "InvChild");

            migrationBuilder.AddColumn<int>(
                name: "ChildInvoiceId",
                table: "InvMaster",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvMaster_ChildInvoiceId",
                table: "InvMaster",
                column: "ChildInvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvMaster_InvChild_ChildInvoiceId",
                table: "InvMaster",
                column: "ChildInvoiceId",
                principalTable: "InvChild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
