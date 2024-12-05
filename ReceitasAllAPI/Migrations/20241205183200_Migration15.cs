using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceitasAllAPI.Migrations
{
    /// <inheritdoc />
    public partial class Migration15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cookbooks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cookbooks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cookbooks_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecipeCookbooks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    CookbookId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeCookbooks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecipeCookbooks_Cookbooks_CookbookId",
                        column: x => x.CookbookId,
                        principalTable: "Cookbooks",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_RecipeCookbooks_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cookbooks_AuthorId",
                table: "Cookbooks",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCookbooks_CookbookId",
                table: "RecipeCookbooks",
                column: "CookbookId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCookbooks_RecipeId",
                table: "RecipeCookbooks",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeCookbooks");

            migrationBuilder.DropTable(
                name: "Cookbooks");
        }
    }
}
