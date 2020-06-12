using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureMediaStreaming.Migrations
{
    public partial class moreFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "AssetMetaDataEntities",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    AssetEntityId = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Street = table.Column<string>(maxLength: 100, nullable: true),
                    ZipCode = table.Column<string>(maxLength: 100, nullable: true),
                    City = table.Column<string>(maxLength: 100, nullable: true),
                    State = table.Column<string>(maxLength: 100, nullable: true),
                    Date = table.Column<DateTimeOffset>(nullable: true),
                    Time = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetMetaDataEntities", x => x.Id);
                    table.ForeignKey(
                        "FK_AssetMetaDataEntities_AssetEntities_AssetEntityId",
                        x => x.AssetEntityId,
                        "AssetEntities",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_AssetMetaDataEntities_AssetEntityId",
                "AssetMetaDataEntities",
                "AssetEntityId",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_AssetMetaDataEntities_Id",
                "AssetMetaDataEntities",
                "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "AssetMetaDataEntities");
        }
    }
}