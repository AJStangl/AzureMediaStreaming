using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureMediaStreaming.Migrations
{
    public partial class InitalCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    FileName = table.Column<string>(maxLength: 100, nullable: true),
                    AssetName = table.Column<string>(maxLength: 100, nullable: true),
                    InputAssetName = table.Column<string>(maxLength: 100, nullable: true),
                    OutputAssetName = table.Column<string>(maxLength: 100, nullable: true),
                    JobName = table.Column<string>(maxLength: 100, nullable: true),
                    LocatorName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetEntities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreamingUrls",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    AssetEntityId = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamingUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StreamingUrls_AssetEntities_AssetEntityId",
                        column: x => x.AssetEntityId,
                        principalTable: "AssetEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetEntities_Id",
                table: "AssetEntities",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StreamingUrls_AssetEntityId",
                table: "StreamingUrls",
                column: "AssetEntityId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamingUrls");

            migrationBuilder.DropTable(
                name: "AssetEntities");
        }
    }
}
