using Microsoft.EntityFrameworkCore.Migrations;

namespace MorePracticeMalodyServer.Migrations
{
    public partial class UpdateSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_Title_Artist_Mode",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Charts_Type_Mode_Level",
                table: "Charts");

            migrationBuilder.AddColumn<string>(
                name: "OriginalArtist",
                table: "Songs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalSearchString",
                table: "Songs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SearchSting",
                table: "Songs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Songs_Mode",
                table: "Songs",
                column: "Mode");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_OriginalSearchString",
                table: "Songs",
                column: "OriginalSearchString");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_SearchSting",
                table: "Songs",
                column: "SearchSting");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_Level",
                table: "Charts",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_Mode",
                table: "Charts",
                column: "Mode");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_Type",
                table: "Charts",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Songs_Mode",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_OriginalSearchString",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Songs_SearchSting",
                table: "Songs");

            migrationBuilder.DropIndex(
                name: "IX_Charts_Level",
                table: "Charts");

            migrationBuilder.DropIndex(
                name: "IX_Charts_Mode",
                table: "Charts");

            migrationBuilder.DropIndex(
                name: "IX_Charts_Type",
                table: "Charts");

            migrationBuilder.DropColumn(
                name: "OriginalArtist",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "OriginalSearchString",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "SearchSting",
                table: "Songs");

            migrationBuilder.CreateIndex(
                name: "IX_Songs_Title_Artist_Mode",
                table: "Songs",
                columns: new[] { "Title", "Artist", "Mode" });

            migrationBuilder.CreateIndex(
                name: "IX_Charts_Type_Mode_Level",
                table: "Charts",
                columns: new[] { "Type", "Mode", "Level" });
        }
    }
}
