using Microsoft.EntityFrameworkCore.Migrations;

namespace MorePracticeMalodyServer.Migrations
{
    public partial class UpdateStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventCharts_Songs_SongId",
                table: "EventCharts");

            migrationBuilder.DropIndex(
                name: "IX_EventCharts_SongId",
                table: "EventCharts");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "EventCharts");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Active",
                table: "Events",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_Downloads_Hash",
                table: "Downloads",
                column: "Hash",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_Active",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Downloads_Hash",
                table: "Downloads");

            migrationBuilder.AddColumn<int>(
                name: "SongId",
                table: "EventCharts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventCharts_SongId",
                table: "EventCharts",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventCharts_Songs_SongId",
                table: "EventCharts",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "SongId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
