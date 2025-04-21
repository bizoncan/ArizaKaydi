using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_deleteb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Errors_Users_userId",
                table: "Errors");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageCollection_Errors_errorId",
                table: "ImageCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Users_userId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_Users_userId",
                table: "Works");

            migrationBuilder.AddForeignKey(
                name: "FK_Errors_Users_userId",
                table: "Errors",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageCollection_Errors_errorId",
                table: "ImageCollection",
                column: "errorId",
                principalTable: "Errors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Users_userId",
                table: "WorkOrders",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Users_userId",
                table: "Works",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Errors_Users_userId",
                table: "Errors");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageCollection_Errors_errorId",
                table: "ImageCollection");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Users_userId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_Users_userId",
                table: "Works");

            migrationBuilder.AddForeignKey(
                name: "FK_Errors_Users_userId",
                table: "Errors",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageCollection_Errors_errorId",
                table: "ImageCollection",
                column: "errorId",
                principalTable: "Errors",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Users_userId",
                table: "WorkOrders",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_Users_userId",
                table: "Works",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
