using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AutoCrudAdmin.Demo.SqlServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OpenDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutionType = table.Column<int>(type: "int", nullable: false),
                    LabelType = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTasks_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "DueDate", "Name", "OpenDate" },
                values: new object[] { 1, new DateTime(2022, 1, 1, 12, 3, 25, 0, DateTimeKind.Unspecified), "Setup migration to PostgreSQL", new DateTime(2021, 11, 10, 10, 46, 11, 44, DateTimeKind.Local).AddTicks(2230) });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "DueDate", "Name", "OpenDate" },
                values: new object[] { 2, new DateTime(2021, 10, 4, 12, 3, 25, 0, DateTimeKind.Unspecified), "Update packages", new DateTime(2021, 11, 10, 10, 46, 11, 51, DateTimeKind.Local).AddTicks(3770) });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "DueDate", "Name", "OpenDate" },
                values: new object[] { 3, new DateTime(2021, 11, 8, 12, 3, 25, 0, DateTimeKind.Unspecified), "Integrate AutoCrudAdmin", new DateTime(2021, 11, 10, 10, 46, 11, 51, DateTimeKind.Local).AddTicks(3940) });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "DueDate", "ExecutionType", "LabelType", "Name", "OpenDate", "ProjectId" },
                values: new object[,]
                {
                    { 1, new DateTime(2021, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 0, "Check incompatible entities", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 12, new DateTime(2021, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0, "Change Db connection", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, new DateTime(2022, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, "Setup PostgreSQL server", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, new DateTime(2021, 10, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 0, "Update all packages", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 5, new DateTime(2021, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0, "Install AutoCrudAdmin", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 6, new DateTime(2021, 11, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0, "Setup AutoCrudAdmin", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTasks_EmployeeId",
                table: "EmployeeTasks",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTasks_TaskId",
                table: "EmployeeTasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeTasks");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
