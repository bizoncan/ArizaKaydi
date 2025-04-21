using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_work30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_MachineParts_machniePartId",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "machniePartId",
                table: "Works",
                newName: "machinePartId");

            migrationBuilder.RenameIndex(
                name: "IX_Works_machniePartId",
                table: "Works",
                newName: "IX_Works_machinePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_MachineParts_machinePartId",
                table: "Works",
                column: "machinePartId",
                principalTable: "MachineParts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_MachineParts_machinePartId",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "machinePartId",
                table: "Works",
                newName: "machniePartId");

            migrationBuilder.RenameIndex(
                name: "IX_Works_machinePartId",
                table: "Works",
                newName: "IX_Works_machniePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_MachineParts_machniePartId",
                table: "Works",
                column: "machniePartId",
                principalTable: "MachineParts",
                principalColumn: "Id");
        }
    }
}
