using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PRIME.Core.Web.Migrations
{
    public partial class PRIME01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CDSClientId",
                table: "DSSModels",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CDSClientId",
                table: "AggrModels",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CDSClients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CDSClients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DSSModels_CDSClientId",
                table: "DSSModels",
                column: "CDSClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AggrModels_CDSClientId",
                table: "AggrModels",
                column: "CDSClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AggrModels_CDSClients_CDSClientId",
                table: "AggrModels",
                column: "CDSClientId",
                principalTable: "CDSClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DSSModels_CDSClients_CDSClientId",
                table: "DSSModels",
                column: "CDSClientId",
                principalTable: "CDSClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AggrModels_CDSClients_CDSClientId",
                table: "AggrModels");

            migrationBuilder.DropForeignKey(
                name: "FK_DSSModels_CDSClients_CDSClientId",
                table: "DSSModels");

            migrationBuilder.DropTable(
                name: "CDSClients");

            migrationBuilder.DropIndex(
                name: "IX_DSSModels_CDSClientId",
                table: "DSSModels");

            migrationBuilder.DropIndex(
                name: "IX_AggrModels_CDSClientId",
                table: "AggrModels");

            migrationBuilder.DropColumn(
                name: "CDSClientId",
                table: "DSSModels");

            migrationBuilder.DropColumn(
                name: "CDSClientId",
                table: "AggrModels");
        }
    }
}
