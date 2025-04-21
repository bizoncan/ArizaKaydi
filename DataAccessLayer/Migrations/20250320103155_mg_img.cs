using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_img : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageCollection",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    errorId = table.Column<int>(type: "int", nullable: true),
                    imageDataBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageCollection", x => x.id);
                    table.ForeignKey(
                        name: "FK_ImageCollection_Errors_errorId",
                        column: x => x.errorId,
                        principalTable: "Errors",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageCollection_errorId",
                table: "ImageCollection",
                column: "errorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageCollection");
        }
    }
}
