using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_cascade_ch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineParts_Machines_machineId",
                table: "MachineParts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_MachineParts_machinePartId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Machines_machineId",
                table: "WorkOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineParts_Machines_machineId",
                table: "MachineParts",
                column: "machineId",
                principalTable: "Machines",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_MachineParts_machinePartId",
                table: "WorkOrders",
                column: "machinePartId",
                principalTable: "MachineParts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Machines_machineId",
                table: "WorkOrders",
                column: "machineId",
                principalTable: "Machines",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineParts_Machines_machineId",
                table: "MachineParts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_MachineParts_machinePartId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Machines_machineId",
                table: "WorkOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineParts_Machines_machineId",
                table: "MachineParts",
                column: "machineId",
                principalTable: "Machines",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_MachineParts_machinePartId",
                table: "WorkOrders",
                column: "machinePartId",
                principalTable: "MachineParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Machines_machineId",
                table: "WorkOrders",
                column: "machineId",
                principalTable: "Machines",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
