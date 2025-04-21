using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_workorder18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_MachineParts_machniePartId",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "machniePartId",
                table: "WorkOrders",
                newName: "machinePartId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_machniePartId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_machinePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_MachineParts_machinePartId",
                table: "WorkOrders",
                column: "machinePartId",
                principalTable: "MachineParts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_MachineParts_machinePartId",
                table: "WorkOrders");

            migrationBuilder.RenameColumn(
                name: "machinePartId",
                table: "WorkOrders",
                newName: "machniePartId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_machinePartId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_machniePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_MachineParts_machniePartId",
                table: "WorkOrders",
                column: "machniePartId",
                principalTable: "MachineParts",
                principalColumn: "Id");
        }
    }
}
