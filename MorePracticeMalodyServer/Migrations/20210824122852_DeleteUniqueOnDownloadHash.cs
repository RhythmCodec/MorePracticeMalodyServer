using Microsoft.EntityFrameworkCore.Migrations;

namespace MorePracticeMalodyServer.Migrations
{
    public partial class DeleteUniqueOnDownloadHash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Downloads_Hash",
                table: "Downloads");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Downloads_Hash",
                table: "Downloads",
                column: "Hash",
                unique: true);
        }
    }
}
