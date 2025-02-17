using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace organization_back_end.Migrations
{
    /// <inheritdoc />
    public partial class Addedurltofile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Url",
                table: "Files");
        }
    }
}
