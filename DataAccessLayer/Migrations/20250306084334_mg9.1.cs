using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg91 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MachinePartsError",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    machinePartId = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachinePartsError", x => x.id);
                    table.ForeignKey(
                        name: "FK_MachinePartsError_MachineParts_machinePartId",
                        column: x => x.machinePartId,
                        principalTable: "MachineParts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MachinePartsError_machinePartId",
                table: "MachinePartsError",
                column: "machinePartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachinePartsError");
        }
    }
}
