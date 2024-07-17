using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniverssellePeintureApi.Migrations
{
    public partial class AddPourcentageVenteToProduit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortFeuilleClient_Commerces_CommercantId",
                table: "PortFeuilleClient");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Produits_ProduitId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ProduitId",
                table: "Stocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PortFeuilleClient",
                table: "PortFeuilleClient");

            migrationBuilder.DropColumn(
                name: "ProduitId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "PrixVente",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "valeur_actuel",
                table: "Produits");

            migrationBuilder.RenameTable(
                name: "PortFeuilleClient",
                newName: "portFeuilleClients");

            migrationBuilder.RenameColumn(
                name: "Quantite",
                table: "Stocks",
                newName: "Quantity");

            migrationBuilder.RenameIndex(
                name: "IX_PortFeuilleClient_CommercantId",
                table: "portFeuilleClients",
                newName: "IX_portFeuilleClients_CommercantId");

            migrationBuilder.RenameIndex(
                name: "IX_PortFeuilleClient_Code",
                table: "portFeuilleClients",
                newName: "IX_portFeuilleClients_Code");

            migrationBuilder.AddColumn<decimal>(
                name: "PrixDeVenteTotal",
                table: "Stocks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Produits",
                type: "nvarchar(250)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PourcentageVente",
                table: "Produits",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrixActuel",
                table: "Produits",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StockActuel",
                table: "Produits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "stock",
                table: "Produits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_portFeuilleClients",
                table: "portFeuilleClients",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "StockProduits",
                columns: table => new
                {
                    StockId = table.Column<int>(type: "int", nullable: false),
                    ProduitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockProduits", x => new { x.StockId, x.ProduitId });
                    table.ForeignKey(
                        name: "FK_StockProduits_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockProduits_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockProduits_ProduitId",
                table: "StockProduits",
                column: "ProduitId");

            migrationBuilder.AddForeignKey(
                name: "FK_portFeuilleClients_Commerces_CommercantId",
                table: "portFeuilleClients",
                column: "CommercantId",
                principalTable: "Commerces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_portFeuilleClients_Commerces_CommercantId",
                table: "portFeuilleClients");

            migrationBuilder.DropTable(
                name: "StockProduits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_portFeuilleClients",
                table: "portFeuilleClients");

            migrationBuilder.DropColumn(
                name: "PrixDeVenteTotal",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "PourcentageVente",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "PrixActuel",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "StockActuel",
                table: "Produits");

            migrationBuilder.DropColumn(
                name: "stock",
                table: "Produits");

            migrationBuilder.RenameTable(
                name: "portFeuilleClients",
                newName: "PortFeuilleClient");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Stocks",
                newName: "Quantite");

            migrationBuilder.RenameIndex(
                name: "IX_portFeuilleClients_CommercantId",
                table: "PortFeuilleClient",
                newName: "IX_PortFeuilleClient_CommercantId");

            migrationBuilder.RenameIndex(
                name: "IX_portFeuilleClients_Code",
                table: "PortFeuilleClient",
                newName: "IX_PortFeuilleClient_Code");

            migrationBuilder.AddColumn<int>(
                name: "ProduitId",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Produits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "Produits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PrixVente",
                table: "Produits",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "valeur_actuel",
                table: "Produits",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PortFeuilleClient",
                table: "PortFeuilleClient",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProduitId",
                table: "Stocks",
                column: "ProduitId");

            migrationBuilder.AddForeignKey(
                name: "FK_PortFeuilleClient_Commerces_CommercantId",
                table: "PortFeuilleClient",
                column: "CommercantId",
                principalTable: "Commerces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Produits_ProduitId",
                table: "Stocks",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
