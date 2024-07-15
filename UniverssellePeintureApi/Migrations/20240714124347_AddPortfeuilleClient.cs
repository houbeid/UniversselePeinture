using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniverssellePeintureApi.Migrations
{
    public partial class AddPortfeuilleClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PrixVente",
                table: "Produits",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "valeur_actuel",
                table: "Produits",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PortFeuilleClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    zone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    depot = table.Column<DateTime>(type: "datetime2", nullable: true),
                    visit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    currentPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PriceCompta = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PricePayer = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CommercantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortFeuilleClient", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortFeuilleClient_Commerces_CommercantId",
                        column: x => x.CommercantId,
                        principalTable: "Commerces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortFeuilleClient_Code",
                table: "PortFeuilleClient",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortFeuilleClient_CommercantId",
                table: "PortFeuilleClient",
                column: "CommercantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortFeuilleClient");

            migrationBuilder.DropColumn(
                name: "valeur_actuel",
                table: "Produits");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrixVente",
                table: "Produits",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
