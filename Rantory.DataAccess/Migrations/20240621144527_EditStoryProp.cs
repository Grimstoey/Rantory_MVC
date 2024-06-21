using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rantory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class EditStoryProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoryId",
                table: "Chapters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_StoryId",
                table: "Chapters",
                column: "StoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Stories_StoryId",
                table: "Chapters",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Stories_StoryId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_StoryId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "StoryId",
                table: "Chapters");
        }
    }
}
