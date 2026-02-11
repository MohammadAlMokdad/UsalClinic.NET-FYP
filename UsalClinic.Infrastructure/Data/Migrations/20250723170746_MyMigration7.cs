using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsalClinic.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class MyMigration7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shifts_AspNetUsers_StaffId",
                table: "shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shifts",
                table: "shifts");

            migrationBuilder.RenameTable(
                name: "shifts",
                newName: "Shifts");

            migrationBuilder.RenameIndex(
                name: "IX_shifts_StaffId",
                table: "Shifts",
                newName: "IX_Shifts_StaffId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_StaffId",
                table: "Shifts",
                column: "StaffId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_StaffId",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.RenameTable(
                name: "Shifts",
                newName: "shifts");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_StaffId",
                table: "shifts",
                newName: "IX_shifts_StaffId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shifts",
                table: "shifts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_shifts_AspNetUsers_StaffId",
                table: "shifts",
                column: "StaffId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
