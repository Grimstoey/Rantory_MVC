using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rantory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterNameSources_Chapters_ChapterId",
                table: "ChapterNameSources");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterNameSources_Stories_StoryId",
                table: "ChapterNameSources");

            migrationBuilder.AlterColumn<int>(
                name: "StoryId",
                table: "ChapterNameSources",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ChapterId",
                table: "ChapterNameSources",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterNameSources_Chapters_ChapterId",
                table: "ChapterNameSources",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterNameSources_Stories_StoryId",
                table: "ChapterNameSources",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterNameSources_Chapters_ChapterId",
                table: "ChapterNameSources");

            migrationBuilder.DropForeignKey(
                name: "FK_ChapterNameSources_Stories_StoryId",
                table: "ChapterNameSources");

            migrationBuilder.AlterColumn<int>(
                name: "StoryId",
                table: "ChapterNameSources",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChapterId",
                table: "ChapterNameSources",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterNameSources_Chapters_ChapterId",
                table: "ChapterNameSources",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterNameSources_Stories_StoryId",
                table: "ChapterNameSources",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
