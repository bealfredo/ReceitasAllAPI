using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceitasAllAPI.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_Authors_AuthorID",
                table: "Recipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipe",
                table: "Recipe");

            migrationBuilder.RenameTable(
                name: "Recipe",
                newName: "Recipes");

            migrationBuilder.RenameColumn(
                name: "AuthorID",
                table: "Recipes",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Recipe_AuthorID",
                table: "Recipes",
                newName: "IX_Recipes_AuthorId");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Authors_AuthorId",
                table: "Recipes",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Authors_AuthorId",
                table: "Recipes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Recipes",
                table: "Recipes");

            migrationBuilder.RenameTable(
                name: "Recipes",
                newName: "Recipe");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Recipe",
                newName: "AuthorID");

            migrationBuilder.RenameIndex(
                name: "IX_Recipes_AuthorId",
                table: "Recipe",
                newName: "IX_Recipe_AuthorID");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorID",
                table: "Recipe",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Recipe",
                table: "Recipe",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipe_Authors_AuthorID",
                table: "Recipe",
                column: "AuthorID",
                principalTable: "Authors",
                principalColumn: "ID");
        }
    }
}
