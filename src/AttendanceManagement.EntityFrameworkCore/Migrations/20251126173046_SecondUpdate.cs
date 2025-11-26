using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceManagement.Migrations
{
    /// <inheritdoc />
    public partial class SecondUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ExceptionRequests_RelatedExceptionRequestId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "GroupMemberships");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CreationTime",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Workflows",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Employees",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_EmployeeId",
                table: "Workflows",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_GroupId",
                table: "Employees",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Groups_GroupId",
                table: "Employees",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ExceptionRequests_RelatedExceptionRequestId",
                table: "Notifications",
                column: "RelatedExceptionRequestId",
                principalTable: "ExceptionRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workflows_Employees_EmployeeId",
                table: "Workflows",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Groups_GroupId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ExceptionRequests_RelatedExceptionRequestId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Workflows_Employees_EmployeeId",
                table: "Workflows");

            migrationBuilder.DropIndex(
                name: "IX_Workflows_EmployeeId",
                table: "Workflows");

            migrationBuilder.DropIndex(
                name: "IX_Employees_GroupId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Workflows");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "GroupMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMemberships_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMemberships_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreationTime",
                table: "Notifications",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberships_EmployeeId",
                table: "GroupMemberships",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberships_GroupId_EmployeeId",
                table: "GroupMemberships",
                columns: new[] { "GroupId", "EmployeeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ExceptionRequests_RelatedExceptionRequestId",
                table: "Notifications",
                column: "RelatedExceptionRequestId",
                principalTable: "ExceptionRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
