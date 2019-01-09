using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EStalls.Data.Migrations
{
    public partial class AddPurchasedItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchasedItem",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    PurchaseDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasedItem", x => new { x.UserId, x.ItemId });
                    table.UniqueConstraint("AK_PurchasedItem_ItemId_UserId", x => new { x.ItemId, x.UserId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchasedItem");
        }
    }
}
