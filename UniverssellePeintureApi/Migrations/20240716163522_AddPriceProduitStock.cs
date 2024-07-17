using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniverssellePeintureApi.Migrations
{
    public partial class AddPriceProduitStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "prix_actuell",
                table: "StockProduits",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "prix_vent",
                table: "StockProduits",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "prix_actuell",
                table: "StockProduits");

            migrationBuilder.DropColumn(
                name: "prix_vent",
                table: "StockProduits");
        }
    }
}
