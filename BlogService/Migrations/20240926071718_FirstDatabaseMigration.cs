using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogService.Migrations
{
    /// <inheritdoc />
    public partial class FirstDatabaseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebsiteDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogTopics = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebsiteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogInfos_WebsiteDatas_WebsiteId",
                        column: x => x.WebsiteId,
                        principalTable: "WebsiteDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebsiteId = table.Column<int>(type: "int", nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogLogs_WebsiteDatas_WebsiteId",
                        column: x => x.WebsiteId,
                        principalTable: "WebsiteDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogInfos_WebsiteId",
                table: "BlogInfos",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogLogs_WebsiteId",
                table: "BlogLogs",
                column: "WebsiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogInfos");

            migrationBuilder.DropTable(
                name: "BlogLogs");

            migrationBuilder.DropTable(
                name: "WebsiteDatas");
        }
    }
}
