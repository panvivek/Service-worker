using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceWorkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class updatedbookingmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Booking_TimeSlotId",
                table: "Booking",
                column: "TimeSlotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booking_TimeSlot_List_TimeSlotId",
                table: "Booking",
                column: "TimeSlotId",
                principalTable: "TimeSlot_List",
                principalColumn: "TimeSlotId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booking_TimeSlot_List_TimeSlotId",
                table: "Booking");

            migrationBuilder.DropIndex(
                name: "IX_Booking_TimeSlotId",
                table: "Booking");
        }
    }
}
