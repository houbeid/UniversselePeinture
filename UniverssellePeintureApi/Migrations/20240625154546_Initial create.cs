using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniverssellePeintureApi.Migrations
{
    public partial class Initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Name_Society = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone_Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Respnsible_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gérant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Solvabilité = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoordonnéesGPS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Recommandation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visit_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivery_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
