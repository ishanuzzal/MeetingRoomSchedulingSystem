using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class pkadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Properties_MinTimeLimit_MaxTimeLimit",
                table: "MeetingTimeLimit");

            migrationBuilder.RenameTable(
                name: "MeetingTimeLimit",
                newName: "MeetingTimeLimits");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MeetingTimeLimits",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingTimeLimits",
                table: "MeetingTimeLimits",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingTimeLimits",
                table: "MeetingTimeLimits");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MeetingTimeLimits");

            migrationBuilder.RenameTable(
                name: "MeetingTimeLimits",
                newName: "MeetingTimeLimit");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Properties_MinTimeLimit_MaxTimeLimit",
                table: "MeetingTimeLimit",
                sql: "[MinimumMinuteTime]<[MaximumMinuteTime]");
        }
    }
}
