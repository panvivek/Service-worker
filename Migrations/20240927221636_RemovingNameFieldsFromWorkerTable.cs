using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceWorkerWebsite.Migrations
{
    /// <inheritdoc />
    public partial class RemovingNameFieldsFromWorkerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Worker_List");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Worker_List",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
