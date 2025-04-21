using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_work25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageCollection_Works_workId",
                table: "ImageCollection");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageCollection_Works_workId",
                table: "ImageCollection",
                column: "workId",
                principalTable: "Works",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageCollection_Works_workId",
                table: "ImageCollection");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageCollection_Works_workId",
                table: "ImageCollection",
                column: "workId",
                principalTable: "Works",
                principalColumn: "id");
        }
    }
}
