using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceWorkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCustomrColumefromBookingPage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerContact",
                table: "Booking");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Booking");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Worker_List",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worker_List_RoleId",
                table: "Worker_List",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Worker_List_AspNetRoles_RoleId",
                table: "Worker_List",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Worker_List_AspNetRoles_RoleId",
                table: "Worker_List");

            migrationBuilder.DropIndex(
                name: "IX_Worker_List_RoleId",
                table: "Worker_List");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Worker_List");

            migrationBuilder.AddColumn<string>(
                name: "CustomerContact",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Booking",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
