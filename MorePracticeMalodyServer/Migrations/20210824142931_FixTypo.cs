using Microsoft.EntityFrameworkCore.Migrations;

namespace MorePracticeMalodyServer.Migrations
{
    public partial class FixTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SearchSting",
                table: "Songs",
                newName: "SearchString");

            migrationBuilder.RenameIndex(
                name: "IX_Songs_SearchSting",
                table: "Songs",
                newName: "IX_Songs_SearchString");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SearchString",
                table: "Songs",
                newName: "SearchSting");

            migrationBuilder.RenameIndex(
                name: "IX_Songs_SearchString",
                table: "Songs",
                newName: "IX_Songs_SearchSting");
        }
    }
}
