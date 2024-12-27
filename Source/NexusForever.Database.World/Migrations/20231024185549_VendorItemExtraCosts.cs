using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.World.Migrations
{
    /// <inheritdoc />
    public partial class VendorItemExtraCosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "extraCost1ItemOrCurrencyId",
                table: "entity_vendor_item",
                type: "int(10) unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "extraCost1Quantity",
                table: "entity_vendor_item",
                type: "int(10) unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "extraCost1Type",
                table: "entity_vendor_item",
                type: "tinyint(3) unsigned",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<uint>(
                name: "extraCost2ItemOrCurrencyId",
                table: "entity_vendor_item",
                type: "int(10) unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "extraCost2Quantity",
                table: "entity_vendor_item",
                type: "int(10) unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<byte>(
                name: "extraCost2Type",
                table: "entity_vendor_item",
                type: "tinyint(3) unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "extraCost1ItemOrCurrencyId",
                table: "entity_vendor_item");

            migrationBuilder.DropColumn(
                name: "extraCost1Quantity",
                table: "entity_vendor_item");

            migrationBuilder.DropColumn(
                name: "extraCost1Type",
                table: "entity_vendor_item");

            migrationBuilder.DropColumn(
                name: "extraCost2ItemOrCurrencyId",
                table: "entity_vendor_item");

            migrationBuilder.DropColumn(
                name: "extraCost2Quantity",
                table: "entity_vendor_item");

            migrationBuilder.DropColumn(
                name: "extraCost2Type",
                table: "entity_vendor_item");
        }
    }
}
