using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class mg72 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MachineNotifications_machineId",
                table: "MachineNotifications",
                column: "machineId");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineNotifications_Machines_machineId",
                table: "MachineNotifications",
                column: "machineId",
                principalTable: "Machines",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineNotifications_Machines_machineId",
                table: "MachineNotifications");

            migrationBuilder.DropIndex(
                name: "IX_MachineNotifications_machineId",
                table: "MachineNotifications");
        }
    }
}
