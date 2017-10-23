using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PharmaBook.Migrations
{
    public partial class invocetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChildInvoice_MstrInvc_MasterInvoiceId",
                table: "ChildInvoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MstrInvc",
                table: "MstrInvc");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChildInvoice",
                table: "ChildInvoice");

            migrationBuilder.DropIndex(
                name: "IX_ChildInvoice_MasterInvoiceId",
                table: "ChildInvoice");

            migrationBuilder.DropColumn(
                name: "MasterId_idnty",
                table: "ChildInvoice");

            migrationBuilder.DropColumn(
                name: "MasterInvoiceId",
                table: "ChildInvoice");

            migrationBuilder.RenameTable(
                name: "MstrInvc",
                newName: "InvMaster");

            migrationBuilder.RenameTable(
                name: "ChildInvoice",
                newName: "InvChild");

            migrationBuilder.RenameColumn(
                name: "Rs",
                table: "InvChild",
                newName: "Amount");

            migrationBuilder.AddColumn<int>(
                name: "ChildInvoiceId",
                table: "InvMaster",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvMaster",
                table: "InvMaster",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvChild",
                table: "InvChild",
                column: "Id");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvMaster_InvChild_ChildInvoiceId",
                table: "InvMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvMaster",
                table: "InvMaster");

            migrationBuilder.DropIndex(
                name: "IX_InvMaster_ChildInvoiceId",
                table: "InvMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvChild",
                table: "InvChild");

            migrationBuilder.DropColumn(
                name: "ChildInvoiceId",
                table: "InvMaster");

            migrationBuilder.RenameTable(
                name: "InvMaster",
                newName: "MstrInvc");

            migrationBuilder.RenameTable(
                name: "InvChild",
                newName: "ChildInvoice");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "ChildInvoice",
                newName: "Rs");

            migrationBuilder.AddColumn<int>(
                name: "MasterId_idnty",
                table: "ChildInvoice",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MasterInvoiceId",
                table: "ChildInvoice",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MstrInvc",
                table: "MstrInvc",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChildInvoice",
                table: "ChildInvoice",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChildInvoice_MasterInvoiceId",
                table: "ChildInvoice",
                column: "MasterInvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChildInvoice_MstrInvc_MasterInvoiceId",
                table: "ChildInvoice",
                column: "MasterInvoiceId",
                principalTable: "MstrInvc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
