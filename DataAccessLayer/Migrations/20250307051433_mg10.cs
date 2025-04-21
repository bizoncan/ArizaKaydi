using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "machinePartId",
                table: "Errors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Errors_machinePartId",
                table: "Errors",
                column: "machinePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Errors_MachineParts_machinePartId",
                table: "Errors",
                column: "machinePartId",
                principalTable: "MachineParts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Errors_MachineParts_machinePartId",
                table: "Errors");

            migrationBuilder.DropIndex(
                name: "IX_Errors_machinePartId",
                table: "Errors");

            migrationBuilder.DropColumn(
                name: "machinePartId",
                table: "Errors");
        }
    }
}
