using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rantory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class userid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_AspNetUsers_ApplicationUserId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_ApplicationUserId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Stories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Stories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stories_ApplicationUserId",
                table: "Stories",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_AspNetUsers_ApplicationUserId",
                table: "Stories",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
