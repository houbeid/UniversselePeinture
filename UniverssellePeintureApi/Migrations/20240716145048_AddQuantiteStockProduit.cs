using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniverssellePeintureApi.Migrations
{
    public partial class AddQuantiteStockProduit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantite",
                table: "StockProduits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantite",
                table: "StockProduits");
        }
    }
}
