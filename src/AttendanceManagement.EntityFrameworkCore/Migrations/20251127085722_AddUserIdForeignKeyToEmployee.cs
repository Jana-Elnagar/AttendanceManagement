using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdForeignKeyToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_AbpUsers_UserId",
                table: "Employees",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_AbpUsers_UserId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_UserId",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId");
        }
    }
}
