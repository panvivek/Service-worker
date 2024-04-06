using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceWorkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services_List",
                columns: table => new
                {
                    Service_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services_List", x => x.Service_Id);
                });

            migrationBuilder.CreateTable(
                name: "Worker_List",
                columns: table => new
                {
                    Worker_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfilePic_Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Availability_Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ratings = table.Column<double>(type: "float", nullable: false),
                    Reviews = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker_List", x => x.Worker_Id);
                });

            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service_Id = table.Column<int>(type: "int", nullable: false),
                    Worker_Id = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgreeToTerms = table.Column<bool>(type: "bit", nullable: false),
                    BookingTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booking_Services_List_Service_Id",
                        column: x => x.Service_Id,
                        principalTable: "Services_List",
                        principalColumn: "Service_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimeSlot_List",
                columns: table => new
                {
                    TimeSlotId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false),
                    Worker_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlot_List", x => x.TimeSlotId);
                    table.ForeignKey(
                        name: "FK_TimeSlot_List_Worker_List_Worker_Id",
                        column: x => x.Worker_Id,
                        principalTable: "Worker_List",
                        principalColumn: "Worker_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkerServices",
                columns: table => new
                {
                    Worker_Id = table.Column<int>(type: "int", nullable: false),
                    Service_Id = table.Column<int>(type: "int", nullable: false),
                    WorkerServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkerServices", x => new { x.Worker_Id, x.Service_Id });
                    table.ForeignKey(
                        name: "FK_WorkerServices_Services_List_Service_Id",
                        column: x => x.Service_Id,
                        principalTable: "Services_List",
                        principalColumn: "Service_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkerServices_Worker_List_Worker_Id",
                        column: x => x.Worker_Id,
                        principalTable: "Worker_List",
                        principalColumn: "Worker_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booking_Service_Id",
                table: "Booking",
                column: "Service_Id");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSlot_List_Worker_Id",
                table: "TimeSlot_List",
                column: "Worker_Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerServices_Service_Id",
                table: "WorkerServices",
                column: "Service_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "TimeSlot_List");

            migrationBuilder.DropTable(
                name: "WorkerServices");

            migrationBuilder.DropTable(
                name: "Services_List");

            migrationBuilder.DropTable(
                name: "Worker_List");
        }
    }
}
