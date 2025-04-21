using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg81 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineNotifications_Machines_machineId",
                table: "MachineNotifications");

            migrationBuilder.AlterColumn<int>(
                name: "machineId",
                table: "MachineNotifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MachineNotifications_Machines_machineId",
                table: "MachineNotifications",
                column: "machineId",
                principalTable: "Machines",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MachineNotifications_Machines_machineId",
                table: "MachineNotifications");

            migrationBuilder.AlterColumn<int>(
                name: "machineId",
                table: "MachineNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MachineNotifications_Machines_machineId",
                table: "MachineNotifications",
                column: "machineId",
                principalTable: "Machines",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
