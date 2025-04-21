using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_workorder16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Works",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    workOrderId = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isClosed = table.Column<bool>(type: "bit", nullable: false),
                    isOpened = table.Column<bool>(type: "bit", nullable: false),
                    workOrderStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    workOrderEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    machineId = table.Column<int>(type: "int", nullable: true),
                    machniePartId = table.Column<int>(type: "int", nullable: true),
                    userId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.id);
                    table.ForeignKey(
                        name: "FK_Works_MachineParts_machniePartId",
                        column: x => x.machniePartId,
                        principalTable: "MachineParts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Works_Machines_machineId",
                        column: x => x.machineId,
                        principalTable: "Machines",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Works_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Works_WorkOrders_workOrderId",
                        column: x => x.workOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Works_machineId",
                table: "Works",
                column: "machineId");

            migrationBuilder.CreateIndex(
                name: "IX_Works_machniePartId",
                table: "Works",
                column: "machniePartId");

            migrationBuilder.CreateIndex(
                name: "IX_Works_userId",
                table: "Works",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Works_workOrderId",
                table: "Works",
                column: "workOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Works");
        }
    }
}
