using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsalClinic.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class MyMigration6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfWeek",
                table: "shifts");

            migrationBuilder.AddColumn<string>(
                name: "DaysOfWeek",
                table: "shifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                table: "shifts");

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeek",
                table: "shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
