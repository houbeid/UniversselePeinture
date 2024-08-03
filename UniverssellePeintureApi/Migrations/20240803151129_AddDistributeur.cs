using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniverssellePeintureApi.Migrations
{
    public partial class AddDistributeur : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "distributeur",
                table: "Historiques",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "distributeur",
                table: "Historiques");
        }
    }
}
