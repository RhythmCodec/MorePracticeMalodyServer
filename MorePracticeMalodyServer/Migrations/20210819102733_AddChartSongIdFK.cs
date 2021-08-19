using Microsoft.EntityFrameworkCore.Migrations;

namespace MorePracticeMalodyServer.Migrations
{
    public partial class AddChartSongIdFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Charts_Songs_SongId",
                table: "Charts");

            migrationBuilder.AlterColumn<int>(
                name: "SongId",
                table: "Charts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Charts_Songs_SongId",
                table: "Charts",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "SongId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Charts_Songs_SongId",
                table: "Charts");

            migrationBuilder.AlterColumn<int>(
                name: "SongId",
                table: "Charts",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Charts_Songs_SongId",
                table: "Charts",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "SongId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
