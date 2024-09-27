using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceWorkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class RemovingReviewFieldsFromWorkerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Availability_Status",
                table: "Worker_List");

            migrationBuilder.DropColumn(
                name: "Ratings",
                table: "Worker_List");

            migrationBuilder.DropColumn(
                name: "Reviews",
                table: "Worker_List");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Availability_Status",
                table: "Worker_List",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Ratings",
                table: "Worker_List",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Reviews",
                table: "Worker_List",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
