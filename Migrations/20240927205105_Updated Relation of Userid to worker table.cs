using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceWorkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationofUseridtoworkertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Worker_List",
                type: "nvarchar(450)",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_Worker_List_UserId",
                table: "Worker_List",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Worker_Id",
                table: "Booking",
                column: "Worker_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_Worker_List_Worker_Id",
                table: "Booking",
                column: "Worker_Id",
                principalTable: "Worker_List",
                principalColumn: "Worker_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Worker_List_AspNetUsers_UserId",
                table: "Worker_List",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_Worker_List_Worker_Id",
                table: "Booking");

            migrationBuilder.DropForeignKey(
                name: "FK_Worker_List_AspNetUsers_UserId",
                table: "Worker_List");


            migrationBuilder.DropIndex(
                name: "IX_Worker_List_UserId",
                table: "Worker_List");

            migrationBuilder.DropIndex(
                name: "IX_Booking_Worker_Id",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Worker_List");

          

        }
    }
}
