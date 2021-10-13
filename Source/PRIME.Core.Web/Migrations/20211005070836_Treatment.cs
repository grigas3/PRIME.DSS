using Microsoft.EntityFrameworkCore.Migrations;

namespace PRIME.Core.Web.Migrations
{
    public partial class Treatment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TreatmentSuggestion",
                table: "DSSModels",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TreatmentSuggestion",
                table: "DSSModels");
        }
    }
}
