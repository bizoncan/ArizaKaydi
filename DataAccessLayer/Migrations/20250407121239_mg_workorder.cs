using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_workorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    desc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isClosed = table.Column<bool>(type: "bit", nullable: false),
                    workOrderStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    workOrderEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    machineId = table.Column<int>(type: "int", nullable: true),
                    machniePartId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_MachineParts_machniePartId",
                        column: x => x.machniePartId,
                        principalTable: "MachineParts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkOrders_Machines_machineId",
                        column: x => x.machineId,
                        principalTable: "Machines",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_machineId",
                table: "WorkOrders",
                column: "machineId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_machniePartId",
                table: "WorkOrders",
                column: "machniePartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrders");
        }
    }
}
