using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReceitasAllAPI.Migrations
{
    /// <inheritdoc />
    public partial class Migration6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Authors",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Authors",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "IsAdmin",
                table: "Authors",
                newName: "Admin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Authors",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Authors",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "Admin",
                table: "Authors",
                newName: "IsAdmin");
        }
    }
}
