using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MeetingTimeLimittableadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeetingTimeLimit",
                columns: table => new
                {
                    MinimumMinuteTime = table.Column<int>(type: "int", nullable: false),
                    MaximumMinuteTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.CheckConstraint("CK_Properties_MinTimeLimit_MaxTimeLimit", "[MinimumMinuteTime]<[MaximumMinuteTime]");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingTimeLimit");
        }
    }
}
