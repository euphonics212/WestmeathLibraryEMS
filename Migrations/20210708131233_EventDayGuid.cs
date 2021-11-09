using Microsoft.EntityFrameworkCore.Migrations;

namespace WestmeathLibraryEMS.Migrations
{
    public partial class EventDayGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "EventDays",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "EventDays");
        }
    }
}
