using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PRIME.Core.Web.Migrations
{
    public partial class _20211007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AggregationPeriodDays",
                table: "DSSModels",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AuthenticationUrl",
                table: "CDSClients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AuthorizationUrl",
                table: "CDSClients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceUrl",
                table: "CDSClients",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DSSModelInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CDSClientId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DSSModelInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DSSModelInfo_CDSClients_CDSClientId",
                        column: x => x.CDSClientId,
                        principalTable: "CDSClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DSSModelInfo_CDSClientId",
                table: "DSSModelInfo",
                column: "CDSClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DSSModelInfo");

            migrationBuilder.DropColumn(
                name: "AggregationPeriodDays",
                table: "DSSModels");

            migrationBuilder.DropColumn(
                name: "AuthenticationUrl",
                table: "CDSClients");

            migrationBuilder.DropColumn(
                name: "AuthorizationUrl",
                table: "CDSClients");

            migrationBuilder.DropColumn(
                name: "ResourceUrl",
                table: "CDSClients");
        }
    }
}
