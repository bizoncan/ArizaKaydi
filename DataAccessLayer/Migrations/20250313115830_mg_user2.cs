using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_user2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "Errors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Errors_userId",
                table: "Errors",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Errors_Users_userId",
                table: "Errors",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Errors_Users_userId",
                table: "Errors");

            migrationBuilder.DropIndex(
                name: "IX_Errors_userId",
                table: "Errors");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Errors");
        }
    }
}
