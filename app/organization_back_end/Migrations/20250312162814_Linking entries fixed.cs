using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace organization_back_end.Migrations
{
    /// <inheritdoc />
    public partial class Linkingentriesfixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkedEntries",
                table: "Entries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkedEntries",
                table: "Entries");
        }
    }
}
