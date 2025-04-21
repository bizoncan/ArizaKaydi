using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_deneme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "machinePartId",
                table: "MachineNotifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MachineNotifications_machinePartId",
                table: "MachineNotifications",
                column: "machinePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineNotifications_MachineParts_machinePartId",
                table: "MachineNotifications",
                column: "machinePartId",
                principalTable: "MachineParts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineNotifications_MachineParts_machinePartId",
                table: "MachineNotifications");

            migrationBuilder.DropIndex(
                name: "IX_MachineNotifications_machinePartId",
                table: "MachineNotifications");

            migrationBuilder.DropColumn(
                name: "machinePartId",
                table: "MachineNotifications");
        }
    }
}
