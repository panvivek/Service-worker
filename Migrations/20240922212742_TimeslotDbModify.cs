using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceWorkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class TimeslotDbModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TimeSlot_List");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TimeSlot_List");

            migrationBuilder.AddColumn<string>(
                name: "SelectedDates",
                table: "TimeSlot_List",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimePeriod",
                table: "TimeSlot_List",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeSlots",
                table: "TimeSlot_List",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelectedDates",
                table: "TimeSlot_List");

            migrationBuilder.DropColumn(
                name: "TimePeriod",
                table: "TimeSlot_List");

            migrationBuilder.DropColumn(
                name: "TimeSlots",
                table: "TimeSlot_List");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "TimeSlot_List",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "TimeSlot_List",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
