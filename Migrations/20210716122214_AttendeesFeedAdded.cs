using Microsoft.EntityFrameworkCore.Migrations;

namespace WestmeathLibraryEMS.Migrations
{
    public partial class AttendeesFeedAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "EventDays");

            migrationBuilder.AddColumn<int>(
                name: "ActualAttendees",
                table: "EventDays",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "EventDays",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualAttendees",
                table: "EventDays");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "EventDays");

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "EventDays",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
