using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class ThirdUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workflows_Employees_EmployeeId",
                table: "Workflows");

            migrationBuilder.DropIndex(
                name: "IX_Workflows_EmployeeId",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Workflows");

            migrationBuilder.AlterColumn<string>(
                name: "FloorNumber",
                table: "ScheduleAssignments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_WorkflowId",
                table: "Employees",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Workflows_WorkflowId",
                table: "Employees",
                column: "WorkflowId",
                principalTable: "Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Workflows_WorkflowId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_WorkflowId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "Employees");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Workflows",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FloorNumber",
                table: "ScheduleAssignments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_EmployeeId",
                table: "Workflows",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workflows_Employees_EmployeeId",
                table: "Workflows",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
