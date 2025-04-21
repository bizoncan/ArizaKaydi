using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class mg_img123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageDataBase64",
                table: "ImageCollection");

            migrationBuilder.AddColumn<byte[]>(
                name: "imageDataByte",
                table: "ImageCollection",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imageDataByte",
                table: "ImageCollection");

            migrationBuilder.AddColumn<string>(
                name: "imageDataBase64",
                table: "ImageCollection",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
