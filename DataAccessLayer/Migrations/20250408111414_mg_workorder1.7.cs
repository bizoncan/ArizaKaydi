using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_workorder17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "workId",
                table: "ImageCollection",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageCollection_workId",
                table: "ImageCollection",
                column: "workId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageCollection_Works_workId",
                table: "ImageCollection",
                column: "workId",
                principalTable: "Works",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageCollection_Works_workId",
                table: "ImageCollection");

            migrationBuilder.DropIndex(
                name: "IX_ImageCollection_workId",
                table: "ImageCollection");

            migrationBuilder.DropColumn(
                name: "workId",
                table: "ImageCollection");
        }
    }
}
