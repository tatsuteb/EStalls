using Microsoft.EntityFrameworkCore.Migrations;

namespace EStalls.Data.Migrations
{
    public partial class AddThumbnailFileNameToItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailFileName",
                table: "Item",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailFileName",
                table: "Item");
        }
    }
}
